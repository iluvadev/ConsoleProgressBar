// Copyright (c) 2020, iluvadev, and released under MIT License.  This can be found in the root of this distribution. 

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgressBar
{
    /// <summary>
    /// A ProgressBar for Console
    /// </summary>
    public class ProgressBar : IDisposable
    {
        /// <summary>
        /// Definition of a Layout for a ProgressBar representation
        /// </summary>
        public class ProgressBarLayout
        {
            //[■■■■■···············] -> ShowProgress
            //[·······■············] -> ShowMarquee
            //[■■■■■··+············] -> ShowProgress + ShowMarquee
            //[■■■■■■■■#■■■········] -> ShowProgress + ShowMarquee (overlapped)

            /// <summary>
            /// String with the 'Starting part' of the ProgressBar
            /// Default = "[" -> [■■■■■···············]
            /// </summary>
            public string Start { get; set; } = "[";

            /// <summary>
            /// String with the 'Ending part' of the ProgressBar
            /// Default = "]" -> [■■■■■···············]
            /// </summary>
            public string End { get; set; } = "]";

            /// <summary>
            /// Char for 'Pending' step (without progress)
            /// Default = '·' -> [■■■■■···············]
            /// </summary>
            public char Pending { get; set; } = '·';

            /// <summary>
            /// Char for 'Progress' step
            /// Default = '■' -> [■■■■■···············]
            /// </summary>
            public char Progress { get; set; } = '■';

            /// <summary>
            /// Char for the 'Marquee' when the ProgressBar don't show Progress 
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '■' -> [·······■············]
            /// </summary>
            public char MarqueeAlone { get; set; } = '■';

            /// <summary>
            /// Char for the 'Marquee' when it moves over a 'Pending' step when ProgressBar show Progress
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '+' -> [■■■■■··+············]
            /// </summary>
            public char MarqueeInProgressPending { get; set; } = '+';

            /// <summary>
            /// Char for the 'Marquee' when it moves over a 'Progress' step when ProgressBar show Progress
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '#' -> [■■■■■■■■#■■■········]
            /// </summary>
            public char MarqueeInProgress { get; set; } = '#';

            /// <summary>
            /// Length of the internal part of the ProgressBar, without Start and End
            /// Default = 28
            /// </summary>
            public int InnerLength { get; set; } = 28;

            /// <summary>
            /// Lenght of entire ProgressBar
            /// </summary>
            public int TotalLengt => InnerLength + Start.Length + End.Length;
        }

        /// <summary>
        /// Layout of the ProgressBar
        /// </summary>
        public ProgressBarLayout Layout { get; set; }

        /// <summary>
        /// The Maximum value
        /// Default = 100
        /// </summary>
        public int Maximum { get; set; } = 100;

        private int _Value = 0;
        /// <summary>
        /// The current Value
        /// If Value is greater than Maximum, then updates Maximum value
        /// </summary>
        public int Value
        {
            get => _Value;
            set => SetValue(value);
        }

        /// <summary>
        /// The amount by which to increment the ProgressBar with each call to the PerformStep() method.
        /// Default = 1
        /// </summary>
        public int Step { get; set; } = 1;

        /// <summary>
        /// True for show the Marquee in the ProgressBar
        /// The Marquee is a char that moves around the ProgressBar
        /// Default = true
        /// </summary>
        public bool ShowMarquee { get; set; } = true;

        /// <summary>
        /// True for show information about Progress in the ProgressBar
        /// Default = true
        /// </summary>
        public bool ShowProgress { get; set; } = true;

        /// <summary>
        /// Delay for repaint all ProgressBar
        /// Default = 100
        /// </summary>
        public int RepaintDelay { get; set; } = 100;

        /// <summary>
        /// The Name of the Curent Element
        /// </summary>
        public string CurrentElementName { get; set; }

        /// <summary>
        /// Percentage of progress
        /// </summary>
        public int Percentage => Maximum != 0 ? ((Value * 100) / Maximum) : 100;

        /// <summary>
        /// True if ProgressBar is Started
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// True if ProgressBar is Paused
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// True if ProgresBar is Done: when disposing or Progress is finished
        /// </summary>
        public bool IsDone => CancelThread || (ShowProgress && Value == Maximum);

        /// <summary>
        /// Processing time (time paused excluded)
        /// </summary>
        public TimeSpan ProcessingTime => ProgressStopwatch.Elapsed;

        /// <summary>
        /// Processing time per element (median)
        /// </summary>
        public TimeSpan TimePerElement => new TimeSpan(TicksPerElement);

        /// <summary>
        /// Estimated time finish (to Value = Maximum)
        /// </summary>
        public TimeSpan? RemainingTime { get; private set; } = null;

        /// <summary>
        /// Function to Convert RemainingTime TimeSpan to String
        /// </summary>
        public Func<TimeSpan?, string> RemainingTimeSpanToStringConverter { get; set; }
            = (ts) =>
            {
                int units;
                if (!ts.HasValue) return "unknown";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalDays))) > 1) return $"{units} days";
                else if ((units = Convert.ToInt32(Math.Floor(ts.Value.TotalDays))) == 1) return $"{units} day";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalHours))) > 1) return $"{units} hours";
                else if ((units = Convert.ToInt32(Math.Floor(ts.Value.TotalHours))) == 1) return $"{units} hour";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalMinutes))) > 1) return $"{units} minutes";
                else if ((units = Convert.ToInt32(Math.Floor(ts.Value.TotalMinutes))) == 1) return $"{units} minute";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalSeconds))) > 1) return $"{units} seconds";
                else if ((units = Convert.ToInt32(Math.Floor(ts.Value.TotalSeconds))) == 1) return $"{units} second";
                else return "a moment";
            };

        /// <summary>
        /// Function to Convert Processing time to String
        /// </summary>
        public Func<TimeSpan, string> ProcessTimeSpanToStringConverter { get; set; }
            = (ts) => $"{ts.TotalHours:F0}{ts:\\:mm\\:ss\\.fff}";

        /// <summary>
        /// Function to Get the Text to put after ProgressBar, when it does not show Progress
        /// </summary>
        public Func<ProgressBar, string> UndefinedProgressTextGetter { get; set; }
            = (pb) => pb.IsPaused ?
                        $"Paused... Running time: {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)}" :
                        $"Processing... ({pb.Value} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)})";

        /// <summary>
        /// Function to Get the Text to put after ProgressBar, when it show Progress
        /// </summary>
        public Func<ProgressBar, string> ProgressTextGetter { get; set; }
            = (pb) => $"{pb.Value} of {pb.Maximum} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)}{(pb.IsPaused ? " (paused)" : $", remaining: {pb.RemainingTimeSpanToStringConverter?.Invoke(pb.RemainingTime)}")}";

        /// <summary>
        /// Function to Get the Text to put after ProgressBar, when it is Done (disposed or finished)
        /// </summary>
        public Func<ProgressBar, string> DoneTextGetter { get; set; }
            = (pb) => $"Done!";

        /// <summary>
        /// Function to Get the Description lines to put under ProgressBar
        /// </summary>
        public Func<ProgressBar, string[]> DescriptionLinesGetter { get; set; }
            = (pb) => new string[] { pb.IsPaused ? " > Paused <" : $" > {pb.CurrentElementName}" };

        /// <summary>
        /// Function to Get the Description lines to put under ProgressBar, when it is Done (disposed or finished)
        /// </summary>
        public Func<ProgressBar, string[]> DoneDescriptionLinesGetter { get; set; }
            = (pb) => new string[] { $" > {pb.Value} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)} ({pb.ProcessTimeSpanToStringConverter?.Invoke(pb.TimePerElement)} each one)" };


        private int MarqueePosition { get; set; } = -1;
        private int MarqueeIncrement { get; set; } = 1;


        private Thread WorkingThread { get; set; }
        private bool CancelThread { get; set; }

        private Stopwatch ProgressStopwatch { get; set; }
        private long TicksPerElement { get; set; }

        private static readonly object ConsoleWriterLock = new object();
        private int _ConsoleRow = -1;

        /// <summary>
        /// Creates an instance of ConsoleProgressBar
        /// </summary>
        /// <param name="autoStart">True if ProgressBar starts automatically</param>
        /// <param name="layout">The layout to use in the ProgressBar</param>
        public ProgressBar(bool autoStart, ProgressBarLayout layout)
        {
            ProgressStopwatch = new Stopwatch();
            Layout = layout;
            if (autoStart)
                Start();
        }
        /// <summary>
        /// Creates an instance of ConsoleProgressBar
        /// </summary>
        /// <param name="layout">The layout to use in the ProgressBar</param>
        public ProgressBar(ProgressBarLayout layout)
            : this(true, layout)
        {
        }
        /// <summary>
        /// Creates an instance of ConsoleProgressBar
        /// </summary>
        /// <param name="autoStart">True if ProgressBar starts automatically</param>
        public ProgressBar(bool autoStart)
            : this(autoStart, new ProgressBarLayout())
        {
        }
        /// <summary>
        /// Creates an instance of ConsoleProgressBar
        /// </summary>
        public ProgressBar()
            : this(true, new ProgressBarLayout())
        {
        }

        /// <summary>
        /// Starts the ProgressBar
        /// </summary>
        public void Start()
        {
            if (IsStarted)
                Resume();
            else
            {
                WorkingThread = new Thread(
                  () =>
                  {
                      ProgressStopwatch.Start();
                      while (!CancelThread)
                      {
                          if (!IsPaused)
                          {
                              UpdateMarqueePosition();
                              PrintProgressBar();
                              Task.Delay(RepaintDelay).Wait();
                          }
                      }
                  })
                {
                    IsBackground = true
                };
                WorkingThread.Start();
            }
        }

        /// <summary>
        /// Pauses the ProgressBar
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            ProgressStopwatch.Stop();
            PrintProgressBar();
        }

        /// <summary>
        /// Resume the ProgresBar
        /// </summary>
        public void Resume()
        {
            ProgressStopwatch.Start();
            IsPaused = false;
            PrintProgressBar();
        }

        private void SetValue(int value)
        {
            if (value > Maximum)
                Maximum = value;
            _Value = value;
            UpdateRemainingTime();
        }

        /// <summary>
        /// Advances the current position of the progress bar by the amount of the Step property
        /// </summary>
        /// <param name="newElementName">The name of the new Element</param>
        public void PerformStep(string newElementName = null)
        {
            CurrentElementName = newElementName;
            Value += Step;
        }

        private void UpdateRemainingTime()
        {
            TicksPerElement = Value > 0 ? (long)Math.Round((decimal)ProgressStopwatch.ElapsedTicks / Value) : ProgressStopwatch.ElapsedTicks;
            RemainingTime = Value == 0 ? null as TimeSpan? : new TimeSpan(TicksPerElement * (Maximum - Value));
        }

        private void PrintProgressBar()
        {
            string progressBar = GetProgressBar();
            string text = "";
            //Text will be printed if there are 10 chars or more
            if (progressBar.Length <= Console.BufferWidth - 10)
            {
                if (IsDone)
                    text = DoneTextGetter?.Invoke(this);
                else if (ShowProgress)
                    text = ProgressTextGetter?.Invoke(this);
                else
                    text = UndefinedProgressTextGetter?.Invoke(this);
            }
            string progressBarLine = progressBar;
            if (!string.IsNullOrEmpty(text))
                progressBarLine += " " + text;

            lock (ConsoleWriterLock)
            {
                if (_ConsoleRow < 0)
                    _ConsoleRow = Console.CursorTop;
                int oldCursorLeft = Console.CursorLeft;
                int oldCursorTop = Console.CursorTop;
                bool oldCursorVisible = Console.CursorVisible;

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, _ConsoleRow);
                Console.WriteLine("\r" + AdaptTextToConsoleWidth(progressBarLine));
                if (IsDone)
                {
                    foreach (var line in DoneDescriptionLinesGetter?.Invoke(this) ?? new string[0])
                        Console.WriteLine(AdaptTextToConsoleWidth(line));
                }
                else
                {
                    foreach (var line in DescriptionLinesGetter?.Invoke(this) ?? new string[0])
                        Console.WriteLine(AdaptTextToConsoleWidth(line));
                }

                if (_ConsoleRow != oldCursorTop)
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
                
                if (oldCursorVisible)
                    Console.CursorVisible = oldCursorVisible;
            }
        }

        private string AdaptTextToConsoleWidth(string value)
        {
            const string append = "...";
            int maxChars = Console.BufferWidth;

            //Truncate to fit in a line
            string textTruncated = value.Length <= maxChars ? value : value.Substring(0, maxChars - append.Length) + append;
            //Add spaces to fill all line
            return textTruncated.PadRight(maxChars);
        }

        private void UpdateMarqueePosition()
        {
            int newProgressPosition = MarqueePosition + MarqueeIncrement;
            if (newProgressPosition < 0 || newProgressPosition >= Layout.InnerLength)
                MarqueeIncrement *= -1;

            MarqueePosition += MarqueeIncrement;
        }

        private string GetProgressBar()
        {
            //[■■■■■···············] -> ShowProgress
            //[·······■············] -> ShowMarquee
            //[■■■■■··+············] -> ShowProgress + ShowMarquee
            //[■■■■■■■■#■■■········] -> ShowProgress + ShowMarquee (overlapped)

            int percentageLenght = ShowProgress ? Convert.ToInt32(Percentage / (100f / Layout.InnerLength)) : 0;

            StringBuilder sb = new StringBuilder();
            sb.Append(Layout.Start);
            for (int i = 0; i < Layout.InnerLength; i++)
            {
                char c;
                if (i == MarqueePosition && ShowMarquee)
                {
                    if (ShowProgress)
                        c = (i < percentageLenght) ? Layout.MarqueeInProgress : Layout.MarqueeInProgressPending;
                    else
                        c = Layout.MarqueeAlone;
                }
                else
                {
                    c = (i < percentageLenght) ? Layout.Progress : Layout.Pending;
                }
                sb.Append(c);
            }
            sb.Append(Layout.End);
            return sb.ToString();
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CancelThread = true;
            ShowMarquee = false;
            UpdateRemainingTime();
            PrintProgressBar();

            if (ProgressStopwatch.IsRunning)
                ProgressStopwatch.Stop();
            ProgressStopwatch.Reset();
        }
    }
}