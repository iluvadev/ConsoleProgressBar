using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgressBar
{
    public class ProgressBarConsole : IDisposable
    {
        public ProgressBarLayout Layout { get; set; }

        public int InnerLength { get; set; } = 28;
        public int TotalLengt => InnerLength + Layout.Start.Length + Layout.End.Length;

        public int Maximum { get; set; } = 100;
        private int _Value = 0;
        public int Value
        {
            get => _Value;
            set => SetValue(value);
        }
        public int Step { get; set; } = 1;

        public bool ShowMarquee { get; set; } = true;
        public bool ShowProgress { get; set; } = true;
        public int MarqueeDelay { get; set; } = 75;

        public string CurrentElementName { get; set; }

        public int Percentage => Maximum != 0 ? ((Value * 100) / Maximum) : 100;
        public bool IsStarted { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsDone => CancelThread || (ShowProgress && Value == Maximum);

        public TimeSpan ProcessingTime => ProgressStopwatch.Elapsed;
        public TimeSpan TimePerElement => new TimeSpan(TicksPerElement);
        public TimeSpan? RemainingTime { get; private set; } = null;


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
        public Func<TimeSpan, string> ProcessTimeSpanToStringConverter { get; set; }
            = (ts) => $"{ts.TotalHours:F0}{ts:\\:mm\\:ss\\.fff}";

        public Func<ProgressBarConsole, string> UndefinedProgressTextGetter { get; set; }
            = (pb) => pb.IsPaused ?
                        $"Paused... Running time: {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)}" :
                        $"Processing... ({pb.Value} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)})";

        public Func<ProgressBarConsole, string> ProgressTextGetter { get; set; }
            = (pb) => $"{pb.Value} of {pb.Maximum} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)}{(pb.IsPaused ? " (paused)" : $", remaining: {pb.RemainingTimeSpanToStringConverter?.Invoke(pb.RemainingTime)}")}";

        public Func<ProgressBarConsole, string[]> MultiLineGetter { get; set; }
            = (pb) => new string[] { pb.IsPaused ? " > Paused <" : $" > {pb.CurrentElementName}" };

        public Func<ProgressBarConsole, string> DoneTextGetter { get; set; }
            = (pb) => $"Done!";

        public Func<ProgressBarConsole, string[]> DoneMultiLineGetter { get; set; }
            = (pb) => new string[] { $" > {pb.Value} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)} ({pb.ProcessTimeSpanToStringConverter?.Invoke(pb.TimePerElement)} each one)" };


        private int MarqueePosition { get; set; } = -1;
        private int MarqueeIncrement { get; set; } = 1;


        private Thread WorkingThread { get; set; }
        private bool CancelThread { get; set; }

        private Stopwatch ProgressStopwatch { get; set; }
        private long TicksPerElement { get; set; }

        private static readonly object ConsoleWriterLock = new object();
        private int _ConsoleRow = -1;

        public ProgressBarConsole(bool autoStart, ProgressBarLayout layout)
        {
            ProgressStopwatch = new Stopwatch();
            Layout = layout;
            if (autoStart)
                Start();
        }
        public ProgressBarConsole(ProgressBarLayout layout)
            : this(true, layout)
        {
        }
        public ProgressBarConsole(bool autoStart)
            : this(autoStart, new ProgressBarLayout())
        {
        }
        public ProgressBarConsole()
            : this(true, new ProgressBarLayout())
        {
        }

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
                              Task.Delay(MarqueeDelay).Wait();
                          }
                      }
                  })
                {
                    IsBackground = true
                };
                WorkingThread.Start();
            }
        }
        public void Pause()
        {
            IsPaused = true;
            ProgressStopwatch.Stop();
            PrintProgressBar();
        }
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

                Console.SetCursorPosition(0, _ConsoleRow);
                Console.WriteLine("\r" + progressBarLine.Truncate(Console.BufferWidth).PadRight(Console.BufferWidth));
                if (IsDone)
                {
                    foreach (var line in DoneMultiLineGetter?.Invoke(this) ?? new string[0])
                        Console.WriteLine(line.Truncate(Console.BufferWidth).PadRight(Console.BufferWidth));
                }
                else
                {
                    foreach (var line in MultiLineGetter?.Invoke(this) ?? new string[0])
                        Console.WriteLine(line.Truncate(Console.BufferWidth).PadRight(Console.BufferWidth));
                }

                if (_ConsoleRow != oldCursorTop)
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
            }
        }


        private void UpdateMarqueePosition()
        {
            int newProgressPosition = MarqueePosition + MarqueeIncrement;
            if (newProgressPosition < 0 || newProgressPosition >= InnerLength)
                MarqueeIncrement *= -1;

            MarqueePosition += MarqueeIncrement;
        }

        private string GetProgressBar()
        {
            //[■■■■■···············] -> ShowProgress
            //[·······■············] -> ShowMarquee
            //[■■■■■··+············] -> ShowProgress + ShowMarquee
            //[■■■■■■■■#■■■········] -> ShowProgress + ShowMarquee (overlapped)

            int percentageLenght = ShowProgress ? Convert.ToInt32(Percentage / (100f / InnerLength)) : 0;

            StringBuilder sb = new StringBuilder();
            sb.Append(Layout.Start);
            for (int i = 0; i < InnerLength; i++)
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