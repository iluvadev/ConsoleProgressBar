// Copyright (c) 2020, iluvadev, and released under MIT License.  This can be found in the root of this distribution. 

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public class Layout
        {
            //[■■■■■···············] -> ShowProgress
            //[·······■············] -> ShowMarquee
            //[■■■■■··+············] -> ShowProgress + ShowMarquee
            //[■■■■■■■■#■■■········] -> ShowProgress + ShowMarquee (overlapped)

            /// <summary>
            /// ColorString with the 'Starting part' of the ProgressBar
            /// Default = "[" -> [■■■■■···············]
            /// </summary>
            public ColorString Start { get; set; } = new ColorString("[", ConsoleColor.DarkBlue);

            /// <summary>
            /// ColorString with the 'Ending part' of the ProgressBar
            /// Default = "]" -> [■■■■■···············]
            /// </summary>
            public ColorString End { get; set; } = new ColorString("]", ConsoleColor.DarkBlue);

            /// <summary>
            /// ColorCharacter for 'Pending' step (without progress)
            /// Default = '·' -> [■■■■■···············]
            /// </summary>
            public ColorCharacter Pending { get; set; } = new ColorCharacter('·', ConsoleColor.DarkGray);

            /// <summary>
            /// ColorCharacter for 'Progress' step
            /// Default = '■' -> [■■■■■···············]
            /// </summary>
            public ColorCharacter Progress { get; set; } = new ColorCharacter('■', ConsoleColor.DarkGreen);

            /// <summary>
            /// ColorCharacter for the 'Marquee' when the ProgressBar don't show Progress 
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '■' -> [·······■············]
            /// </summary>
            public ColorCharacter MarqueeAlone { get; set; } = new ColorCharacter('■', ConsoleColor.Green);

            /// <summary>
            /// ColorCharacter for the 'Marquee' when it moves over a 'Pending' step when ProgressBar show Progress
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '+' -> [■■■■■··+············]
            /// </summary>
            public ColorCharacter MarqueeInProgressPending { get; set; } = new ColorCharacter('·', ConsoleColor.Yellow);

            /// <summary>
            /// ColorCharacter for the 'Marquee' when it moves over a 'Progress' step when ProgressBar show Progress
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '#' -> [■■■■■■■■#■■■········]
            /// </summary>
            public ColorCharacter MarqueeInProgress { get; set; } = new ColorCharacter('■', ConsoleColor.Yellow);

            /// <summary>
            /// Length of the internal part of the ProgressBar, without Start and End
            /// Default = 28
            /// </summary>
            public int InnerLength { get; set; } = 28;

            /// <summary>
            /// Lenght of entire ProgressBar
            /// </summary>
            public int TotalLength => InnerLength + Start.Value.Length + End.Value.Length;

            /// <summary>
            /// Indentation in each DesctiptionLine
            /// Default = " -> "
            /// </summary>
            public ColorString DescriptionLinesIndentation { get; set; } = new ColorString(" -> ", ConsoleColor.DarkBlue);
        }

        /// <summary>
        /// Definition of a Element with colors in Console
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class ColorElement<T>
        {
            /// <summary>
            /// The ForegroundColor
            /// </summary>
            public ConsoleColor? ForegroundColor { get; set; }

            /// <summary>
            /// The BackgroundColor
            /// </summary>
            public ConsoleColor? BackgroundColor { get; set; }

            /// <summary>
            /// The value to represent
            /// </summary>
            public T Value { get; set; }

            public ColorElement(T value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
            {
                Value = value;
                ForegroundColor = foregroundColor;
                BackgroundColor = backgroundColor;
            }

            /// <summary>
            /// Returns a list of Actions to write the element in Console
            /// </summary>
            /// <param name="valueTransformer">Function to Transform the value before write</param>
            /// <returns></returns>
            public List<Action> GetConsoleWriteActions(Func<T, T> valueTransformer = null)
            {
                var list = new List<Action>();
                ConsoleColor? oldForegroundColor = null;
                ConsoleColor? oldBackgroundColor = null;

                if (ForegroundColor.HasValue)
                {
                    list.Add(() => Console.ForegroundColor = ForegroundColor.Value);
                    oldForegroundColor = Console.ForegroundColor;
                }

                if (BackgroundColor.HasValue)
                {
                    list.Add(() => Console.BackgroundColor = BackgroundColor.Value);
                    oldBackgroundColor = Console.BackgroundColor;
                }

                T transformedValue = Value;
                if (valueTransformer != null)
                    transformedValue = valueTransformer.Invoke(Value);
                list.Add(() => Console.Write(transformedValue));

                if (oldForegroundColor.HasValue)
                    list.Add(() => Console.ForegroundColor = oldForegroundColor.Value);
                if (oldBackgroundColor.HasValue)
                    list.Add(() => Console.BackgroundColor = oldBackgroundColor.Value);

                return list;
            }
        }

        /// <summary>
        /// Definition of a Character with colors in Console
        /// </summary>
        public class ColorCharacter : ColorElement<char>
        {
            public ColorCharacter(char value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
                : base(value, foregroundColor, backgroundColor) { }
        }

        /// <summary>
        /// Definition of a String with colors in Console
        /// </summary>
        public class ColorString : ColorElement<string>
        {
            public ColorString(string value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
                : base(value, foregroundColor, backgroundColor) { }
        }

        /// <summary>
        /// Layout of the ProgressBar
        /// </summary>
        public Layout CurrentLayout { get; set; }

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
        /// True to Print the ProgressBar always in last Console Line
        /// False to Print the ProgressBar fixed in Console (Current position at Starting)
        /// You can Write at Console and ProgressBar will always be below your lines
        /// Default = true
        /// </summary>
        public bool KeepInLastLine { get; set; } = true;

        /// <summary>
        /// Delay for repaint all ProgressBar
        /// Default = 75
        /// </summary>
        public int RepaintDelay { get; set; } = 75;

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
        public Func<ProgressBar, ColorString> UndefinedProgressTextGetter { get; set; }
            = (pb) => pb.IsPaused ?
                        new ColorString($"Paused... Running time: {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)}", ConsoleColor.DarkCyan) :
                        new ColorString($"Processing... ({pb.Value} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)})", ConsoleColor.Cyan);

        /// <summary>
        /// Function to Get the Text to put after ProgressBar, when it show Progress
        /// </summary>
        public Func<ProgressBar, ColorString> ProgressTextGetter { get; set; }
            = (pb) => pb.IsPaused ?
                        new ColorString($"{pb.Value} of {pb.Maximum} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)} (paused)", ConsoleColor.DarkCyan) :
                        new ColorString($"{pb.Value} of {pb.Maximum} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)}, remaining: {pb.RemainingTimeSpanToStringConverter?.Invoke(pb.RemainingTime)}", ConsoleColor.Cyan);

        /// <summary>
        /// Function to Get the Text to put after ProgressBar, when it is Done (disposed or finished)
        /// </summary>
        public Func<ProgressBar, ColorString> DoneTextGetter { get; set; }
            = (pb) => new ColorString($"Done!", ConsoleColor.DarkYellow);

        /// <summary>
        /// Function to Get the Description lines to put under ProgressBar
        /// </summary>
        public Func<ProgressBar, IEnumerable<ColorString>> DescriptionLinesGetter { get; set; }
            = (pb) => new ColorString[] { pb.IsPaused ? new ColorString("[Paused]", ConsoleColor.DarkCyan) : new ColorString($"{pb.CurrentElementName}", ConsoleColor.DarkYellow) };

        /// <summary>
        /// Function to Get the Description lines to put under ProgressBar, when it is Done (disposed or finished)
        /// </summary>
        public Func<ProgressBar, IEnumerable<ColorString>> DoneDescriptionLinesGetter { get; set; }
            = (pb) => new ColorString[] { new ColorString($"{pb.Value} in {pb.ProcessTimeSpanToStringConverter?.Invoke(pb.ProcessingTime)} ({pb.ProcessTimeSpanToStringConverter?.Invoke(pb.TimePerElement)} each one)", ConsoleColor.DarkGray) };


        private int MarqueePosition { get; set; } = -1;
        private int MarqueeIncrement { get; set; } = 1;


        private Thread WorkingThread { get; set; }
        private bool CancelThread { get; set; }

        private Stopwatch ProgressStopwatch { get; set; }
        private long TicksPerElement { get; set; }

        public static readonly object ConsoleWriterLock = new object();
        private int _ConsoleRow = -1;
        private int _LastLinesWrited = -1;

        /// <summary>
        /// Creates an instance of ConsoleProgressBar
        /// </summary>
        /// <param name="autoStart">True if ProgressBar starts automatically</param>
        /// <param name="layout">The layout to use in the ProgressBar</param>
        public ProgressBar(bool autoStart = true, Layout layout = null)
        {
            ProgressStopwatch = new Stopwatch();
            CurrentLayout = layout ?? new Layout();
            if (autoStart)
                Start();
        }

        /// <summary>
        /// Starts the ProgressBar
        /// </summary>
        public void Start()
        {
            PrintProgressBar();
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
                              try
                              {
                                  UpdateMarqueePosition();
                                  PrintProgressBar();
                                  Task.Delay(RepaintDelay).Wait();
                              }
                              catch { }
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

        public void RemoveProgressBar()
        {
            if (_ConsoleRow < 0 || _LastLinesWrited <= 0)
                return;

            string emptyLine = AdaptTextToMaxWidth("", Console.BufferWidth);

            //Lock Write to console
            lock (ConsoleWriterLock)
            {
                int oldCursorLeft = Console.CursorLeft;
                int oldCursorTop = Console.CursorTop;
                bool oldCursorVisible = Console.CursorVisible;

                //Hide Cursor
                Console.CursorVisible = false;

                //Position
                Console.SetCursorPosition(0, _ConsoleRow);

                //Remove lines
                for (int i = 0; i < _LastLinesWrited; i++)
                    Console.WriteLine(emptyLine);

                // Restore Cursor Position
                if (_ConsoleRow != oldCursorTop)
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);

                //Show Cursor
                if (oldCursorVisible)
                    Console.CursorVisible = oldCursorVisible;
            }
        }

        public void PrintProgressBar()
        {
            List<Action> actionsProgressBar = GetConsoleActionsForProgressBarAndText();

            //Lock Write to console
            lock (ConsoleWriterLock)
            {
                int oldCursorLeft = Console.CursorLeft;
                int oldCursorTop = Console.CursorTop;
                bool oldCursorVisible = Console.CursorVisible;
                ConsoleColor oldForegroundColor = Console.ForegroundColor;
                ConsoleColor oldBackgroundColor = Console.BackgroundColor;

                //Hide Cursor
                Console.CursorVisible = false;

                //Remove line
                if (KeepInLastLine)
                    Console.WriteLine(AdaptTextToMaxWidth("", Console.BufferWidth));

                //Position
                if (_ConsoleRow < 0)
                    _ConsoleRow = oldCursorTop;
                if (KeepInLastLine)
                    _ConsoleRow = oldCursorTop + 1;
                Console.SetCursorPosition(0, _ConsoleRow);

                //ProgressBar and Text
                actionsProgressBar.ForEach(a => a.Invoke());

                // Restore Cursor Position
                if (_ConsoleRow != oldCursorTop)
                    Console.SetCursorPosition(oldCursorLeft, oldCursorTop);

                //Show Cursor
                if (oldCursorVisible)
                    Console.CursorVisible = oldCursorVisible;

                //Restore colors
                Console.ForegroundColor = oldForegroundColor;
                Console.BackgroundColor = oldBackgroundColor;
            }
        }

        private List<Action> GetConsoleActionsForProgressBarAndText()
        {
            int oldLinesWrited = _LastLinesWrited;

            // ProgressBar
            List<Action> list = GetConsoleActionsForProgressBar(true);

            // Text in same line
            ColorString coloredText = null;
            var maxTextLenght = Console.BufferWidth - CurrentLayout.TotalLength;
            if (maxTextLenght >= 10) //Text will be printed if there are 10 chars or more
            {
                if (IsDone)
                    coloredText = DoneTextGetter?.Invoke(this);
                else if (ShowProgress)
                    coloredText = ProgressTextGetter?.Invoke(this);
                else
                    coloredText = UndefinedProgressTextGetter?.Invoke(this);
                if (!string.IsNullOrEmpty(coloredText?.Value))
                    list.AddRange(coloredText.GetConsoleWriteActions(s => AdaptTextToMaxWidth(" " + s, maxTextLenght)));
            }
            list.Add(() => Console.Write(Environment.NewLine));
            _LastLinesWrited = 1;

            // Description
            var descriptionLinesGetter = IsDone ? DoneDescriptionLinesGetter : DescriptionLinesGetter;
            foreach (var line in descriptionLinesGetter?.Invoke(this) ?? new ColorString[0])
            {
                int indentationLen = CurrentLayout.DescriptionLinesIndentation.Value?.Length ?? 0;
                if (indentationLen > 0)
                    list.AddRange(CurrentLayout.DescriptionLinesIndentation.GetConsoleWriteActions());

                list.AddRange(line.GetConsoleWriteActions(s => AdaptTextToMaxWidth(s, Console.BufferWidth - indentationLen)));
                list.Add(() => Console.Write(Environment.NewLine));
                _LastLinesWrited++;
            }

            // Clear old lines
            if (oldLinesWrited > _LastLinesWrited)
            {
                for (int i = 0; i < _LastLinesWrited - oldLinesWrited; i++)
                    list.Add(() => Console.WriteLine(AdaptTextToMaxWidth("", Console.BufferWidth)));
            }
            return list;
        }

        public static string AdaptTextToMaxWidth(string value, int maxChars)
        {
            const string append = "...";

            //Truncate to fit in a line
            string textTruncated = value.Length <= maxChars ? value : value.Substring(0, maxChars - append.Length) + append;
            //Add spaces to fill all line
            return textTruncated.PadRight(maxChars);
        }

        private void UpdateMarqueePosition()
        {
            int newProgressPosition = MarqueePosition + MarqueeIncrement;
            if (newProgressPosition < 0 || newProgressPosition >= CurrentLayout.InnerLength)
                MarqueeIncrement *= -1;

            MarqueePosition += MarqueeIncrement;
        }

        private List<Action> GetConsoleActionsForProgressBar(bool restoreColors)
        {
            var list = new List<Action>();
            //[■■■■■···············] -> ShowProgress
            //[·······■············] -> ShowMarquee
            //[■■■■■··+············] -> ShowProgress + ShowMarquee
            //[■■■■■■■■#■■■········] -> ShowProgress + ShowMarquee (overlapped)

            ConsoleColor? oldForegroundColor = restoreColors ? Console.ForegroundColor : (ConsoleColor?)null;
            ConsoleColor? oldBackgroundColor = restoreColors ? Console.BackgroundColor : (ConsoleColor?)null;

            int percentageLenght = ShowProgress ? Convert.ToInt32(Percentage / (100f / CurrentLayout.InnerLength)) : 0;

            //Start
            list.AddRange(CurrentLayout.Start.GetConsoleWriteActions());

            //Body
            for (int i = 0; i < CurrentLayout.InnerLength; i++)
            {
                ColorCharacter c;
                if (i == MarqueePosition && ShowMarquee)
                {
                    if (ShowProgress)
                        c = (i < percentageLenght) ? CurrentLayout.MarqueeInProgress : CurrentLayout.MarqueeInProgressPending;
                    else
                        c = CurrentLayout.MarqueeAlone;
                }
                else
                {
                    c = (i < percentageLenght) ? CurrentLayout.Progress : CurrentLayout.Pending;
                }
                list.AddRange(c.GetConsoleWriteActions());
            }

            //End
            list.AddRange(CurrentLayout.End.GetConsoleWriteActions());

            if (restoreColors)
            {
                list.Add(() => Console.ForegroundColor = oldForegroundColor.Value);
                list.Add(() => Console.BackgroundColor = oldBackgroundColor.Value);
            }
            return list;
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
            if (KeepInLastLine && _LastLinesWrited > 0 && _ConsoleRow >= 0)
                Console.CursorTop = _ConsoleRow + _LastLinesWrited; 

            if (ProgressStopwatch.IsRunning)
                    ProgressStopwatch.Stop();
                ProgressStopwatch.Reset();
            }
        }
    }