// Copyright (c) 2020, iluvadev, and released under MIT License.  This can be found in the root of this distribution. 

using System;
using System.Collections.Generic;
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
        /// Helper with Utilities
        /// </summary>
        public static class Utils
        {
            /// <summary>
            /// Converts a nullable TimeSpan to textual Sumarized remaining text: X days, or Y hours, or Z minutes, etc.
            /// </summary>
            /// <param name="ts"></param>
            /// <returns></returns>
            public static string ConvertToStringAsSumarizedRemainingText(TimeSpan? ts)
            {
                int units;
                if (!ts.HasValue) return "unknown";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalDays))) > 1) return $"{units} days";
                else if ((Convert.ToInt32(Math.Floor(ts.Value.TotalDays))) == 1) return $"a day";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalHours))) > 1) return $"{units} hours";
                else if ((Convert.ToInt32(Math.Floor(ts.Value.TotalHours))) == 1) return $"an hour";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalMinutes))) > 1) return $"{units} minutes";
                else if ((Convert.ToInt32(Math.Floor(ts.Value.TotalMinutes))) == 1) return $"a minute";
                else if ((units = Convert.ToInt32(Math.Round(ts.Value.TotalSeconds))) > 1) return $"{units} seconds";
                else if ((Convert.ToInt32(Math.Floor(ts.Value.TotalSeconds))) == 1) return $"a second";
                else return "a moment";
            }

            /// <summary>
            /// Converts a TimeSpan to String, showing all hours
            /// </summary>
            /// <param name="ts"></param>
            /// <param name="includeMilliseconds"></param>
            /// <returns></returns>
            public static string ConvertToStringWithAllHours(TimeSpan? ts, bool includeMilliseconds = true)
            {
                if (!ts.HasValue) return "unknown";
                else if (includeMilliseconds) return $"{ts.Value.TotalHours:F0}{ts.Value:\\:mm\\:ss\\.fff}";
                else return $"{ts.Value.TotalHours:F0}{ts.Value:\\:mm\\:ss}";
            }

            /// <summary>
            /// Gets an string to 'erase' a line in console, this is a line with spaces
            /// </summary>
            /// <returns></returns>
            public static string GetEmptyConsoleLine()
                => "".PadRight(Console.BufferWidth);

            /// <summary>
            /// Returns a string that occupy all console line/s
            /// </summary>
            /// <param name="value">The string to write in console</param>
            /// <param name="allowMultipleLines">To allow print the string in muliple lines or only in one:
            ///     True: The text can be represented in more than one Console line (fill spaces to the end of last line)
            ///     False: The text must be represented in only ONE line (truncate to fit or fill spaces to the end of line)
            /// </param>
            /// <returns></returns>
            public static string AdaptTextToConsole(string value, bool allowMultipleLines = true)
            {
                int maxWidth = Console.BufferWidth;

                if (allowMultipleLines)
                {
                    var lines = Math.DivRem(value.Length, maxWidth, out _) + 1;
                    maxWidth *= lines;
                }
                return AdaptTextToMaxWidth(value, maxWidth);
            }

            /// <summary>
            /// Returns a string with exactly maxChars: Truncates string value or fill with spaces to fits exact length
            /// </summary>
            /// <param name="value"></param>
            /// <param name="maxWidth"></param>
            /// <param name="append">Text appended when it is truncated. Default: "..."</param>
            /// <returns></returns>
            public static string AdaptTextToMaxWidth(string value, int maxWidth, string append = "...")
            {
                int len = value.Length;

                if (len == maxWidth) return value;
                else if (len < maxWidth) return value.PadRight(maxWidth);
                else return value.Substring(0, maxWidth - append.Length) + append;
            }
        }

        /// <summary>
        /// Definition of a Layout for a ProgressBar representation
        /// </summary>
        public class LayoutDefinition
        {
            /*
             *  Examples of ProgressBar
             *
             *      - Marquee is a Character moving around the ProgressBar
             *      
             *      With Progress available (Maximum defined):
             *          [■■■■■■■■■■■■········] -> Without Marquee
             *          [■■■■■■■■■■■■····+···] -> With Marquee (in pending space) 
             *          [■■■■■■■■#■■■········] -> With Marquee (in progress space)
             *          
             *      Without Progress available (don't have Maximum):
             *          [·······■············] -> Marquee is always displayed
             *
             */

            /// <summary>
            /// ColorString with the 'Starting part' of the ProgressBar
            /// Default = "[", DarkBlue -> [■■■■■···············]
            /// </summary>
            public ColorString Start { get; set; } = new ColorString("[", ConsoleColor.DarkBlue);

            /// <summary>
            /// ColorString with the 'Ending part' of the ProgressBar
            /// Default = "]", DarkBlue -> [■■■■■···············]
            /// </summary>
            public ColorString End { get; set; } = new ColorString("]", ConsoleColor.DarkBlue);

            /// <summary>
            /// ColorCharacter for 'Pending' space (without progress)
            /// Default = '·', DarkGray -> [■■■■■···············]
            /// </summary>
            public ColorCharacter Pending { get; set; } = new ColorCharacter('·', ConsoleColor.DarkGray);

            /// <summary>
            /// ColorCharacter for 'Progress' space
            /// Default = '■', DarkGreen -> [■■■■■···············]
            /// </summary>
            public ColorCharacter Progress { get; set; } = new ColorCharacter('■', ConsoleColor.DarkGreen);

            /// <summary>
            /// ColorCharacter for the 'Marquee' when the ProgressBar don't show Progress 
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '■', Green -> [·······■············]
            /// </summary>
            public ColorCharacter MarqueeAlone { get; set; } = new ColorCharacter('■', ConsoleColor.Green);

            /// <summary>
            /// ColorCharacter for the 'Marquee' when it moves over a 'Pending' step when ProgressBar show Progress
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '+', Yellow -> [■■■■■··+············]
            /// </summary>
            public ColorCharacter MarqueeInProgressPending { get; set; } = new ColorCharacter('+', ConsoleColor.Yellow);

            /// <summary>
            /// ColorCharacter for the 'Marquee' when it moves over a 'Progress' step when ProgressBar show Progress
            /// The Marquee is a char that moves around the ProgressBar
            /// Default = '■', Yellow -> [■■■■■■■■#■■■········]
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
            /// Default = " -> ", DarkBlue
            /// </summary>
            public ColorString DescriptionLinesIndentation { get; set; } = new ColorString(" -> ", ConsoleColor.DarkBlue);


            /// <summary>
            /// Function to Get the Text to put after ProgressBar, when it does not show Progress
            /// </summary>
            public Func<ProgressBar, ColorString> NoProgressTextGetter { get; set; }
                = (pb) => pb.IsPaused ?
                            new ColorString($"Paused... Running time: {Utils.ConvertToStringWithAllHours(pb.TimeProcessing)}", ConsoleColor.DarkCyan) :
                            new ColorString($"Processing... ({pb.Value} in {Utils.ConvertToStringWithAllHours(pb.TimeProcessing)})", ConsoleColor.Cyan);

            /// <summary>
            /// Function to Get the Text to put after ProgressBar, when it show Progress
            /// </summary>
            public Func<ProgressBar, ColorString> ProgressTextGetter { get; set; }
                = (pb) => pb.IsPaused ?
                            new ColorString($"{pb.Value} of {pb.Maximum} in {Utils.ConvertToStringWithAllHours(pb.TimeProcessing)} (paused)", ConsoleColor.DarkCyan) :
                            new ColorString($"{pb.Value} of {pb.Maximum} in {Utils.ConvertToStringWithAllHours(pb.TimeProcessing)}, remaining: {Utils.ConvertToStringAsSumarizedRemainingText(pb.TimeRemaining)}", ConsoleColor.Cyan);

            /// <summary>
            /// Function to Get the Text to put after ProgressBar, when it is Done (disposed or finished)
            /// </summary>
            public Func<ProgressBar, ColorString> DoneTextGetter { get; set; }
                = (pb) => new ColorString($"Done!", ConsoleColor.DarkYellow);

            /// <summary>
            /// Function to Get the Description lines to put under ProgressBar
            /// </summary>
            public Func<ProgressBar, IEnumerable<ColorString>> DescriptionLinesGetter { get; set; }
                = (pb) => new ColorString[] { pb.IsPaused ? new ColorString("[Paused]", ConsoleColor.DarkCyan) : new ColorString($"{pb.ElementName}", ConsoleColor.DarkYellow) };

            /// <summary>
            /// Function to Get the Description lines to put under ProgressBar, when it is Done (disposed or finished)
            /// </summary>
            public Func<ProgressBar, IEnumerable<ColorString>> DoneDescriptionLinesGetter { get; set; }
                = (pb) => new ColorString[] { new ColorString($"{pb.Value} in {Utils.ConvertToStringWithAllHours(pb.TimeProcessing)} ({Utils.ConvertToStringWithAllHours(pb.TimePerElement)} each one)", ConsoleColor.DarkGray) };

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
            /// <param name="repetitions">Number of repetitions</param>
            /// <returns></returns>
            public List<Action> GetConsoleWriteActions(Func<T, T> valueTransformer = null, int repetitions = 1)
            {
                var list = new List<Action>();
                if (repetitions < 1)
                    return list;

                if (ForegroundColor.HasValue)
                    list.Add(() => Console.ForegroundColor = ForegroundColor.Value);

                if (BackgroundColor.HasValue)
                    list.Add(() => Console.BackgroundColor = BackgroundColor.Value);

                T transformedValue = Value;
                if (valueTransformer != null)
                    transformedValue = valueTransformer.Invoke(Value);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < repetitions; i++)
                    sb.Append(transformedValue);

                list.Add(() => Console.Write(sb.ToString()));

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
        public LayoutDefinition Layout { get; set; }

        /// <summary>
        /// The Maximum value
        /// Default = 100
        /// </summary>
        public int? Maximum { get; set; } = 100;

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
        /// Percentage of progress
        /// </summary>
        public int? Percentage => Maximum.HasValue ? (Maximum.Value != 0 ? ((Value * 100) / Maximum.Value) : 100) : (int?)null;

        /// <summary>
        /// Indicates if the ProgressBar has Progress defined (Maximum defined)
        /// </summary>
        public bool HasProgress => Maximum.HasValue;

        /// <summary>
        /// The amount by which to increment the ProgressBar with each call to the PerformStep() method.
        /// Default = 1
        /// </summary>
        public int Step { get; set; } = 1;

        /// <summary>
        /// The Name of the Curent Element
        /// </summary>
        public string ElementName { get; set; }


        private bool _ShowMarquee = true;
        /// <summary>
        /// True to show the Marquee in the ProgressBar
        /// The Marquee is a char that moves around the ProgressBar
        /// If there are no Progress to represent (undefined Maximum), Marquee is always Displayed
        /// Default = true
        /// </summary>
        public bool ShowMarquee
        {
            get => !HasProgress || _ShowMarquee;
            set => _ShowMarquee = value;
        }

        private bool _FixedInBottom = false;
        /// <summary>
        /// True to Print the ProgressBar always in last Console Line
        /// False to Print the ProgressBar fixed in Console (Current position at Starting)
        /// You can Write at Console and ProgressBar will always be below your lines
        /// Default = true
        /// </summary>
        public bool FixedInBottom
        {
            get => _FixedInBottom;
            set
            {
                if (_FixedInBottom != value)
                {
                    Unprint();
                    _FixedInBottom = value;
                    Print();
                }
            }
        }

        /// <summary>
        /// Delay for repaint and recalculate all ProgressBar
        /// Default = 75
        /// </summary>
        public int Delay { get; set; } = 75;


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
        public bool IsDone => CancelThread || (HasProgress && Value == Maximum);

        /// <summary>
        /// Processing time (time paused excluded)
        /// </summary>
        public TimeSpan TimeProcessing => ProgressStopwatch.Elapsed;

        /// <summary>
        /// Processing time per element (median)
        /// </summary>
        public TimeSpan? TimePerElement => TicksPerElement.HasValue ? new TimeSpan(TicksPerElement.Value) : (TimeSpan?)null;

        /// <summary>
        /// Estimated time finish (to Value = Maximum)
        /// </summary>
        public TimeSpan? TimeRemaining => TicksRemaining.HasValue ? new TimeSpan(TicksRemaining.Value) : (TimeSpan?)null;

        /// <summary>
        /// A Lock for Writing to Console
        /// </summary>
        public static readonly object ConsoleWriterLock = new object();


        private int MarqueePosition { get; set; } = -1;
        private int MarqueeIncrement { get; set; } = 1;


        private Thread WorkingThread { get; set; }
        private bool CancelThread { get; set; }

        private Stopwatch ProgressStopwatch { get; set; }
        private long? TicksCompletedElements { get; set; }
        private long? TicksPerElement => TicksCompletedElements.HasValue && Value > 0 ? TicksCompletedElements.Value / Value : (long?)null;
        private long? TicksRemaining
        {
            get
            {
                if (!Maximum.HasValue || !TicksPerElement.HasValue) return (long?)null;
                long currentTicks = ProgressStopwatch.ElapsedTicks;
                long totalTicks = TicksPerElement.Value * Maximum.Value;
                return Math.Max(totalTicks - currentTicks, 0);
            }
        }

        private int _ConsoleRow = -1;
        private int _NumberLastLinesWritten = -1;

        /// <summary>
        /// Creates an instance of ConsoleProgressBar
        /// </summary>
        /// <param name="autoStart">True if ProgressBar starts automatically</param>
        /// <param name="layout">The layout to use in the ProgressBar</param>
        public ProgressBar(bool autoStart = true, LayoutDefinition layout = null)
        {
            ProgressStopwatch = new Stopwatch();
            Layout = layout ?? new LayoutDefinition();
            if (autoStart)
                Start();
        }

        /// <summary>
        /// Starts the ProgressBar
        /// </summary>
        public void Start()
        {
            Print();
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
                                  Print();
                                  Task.Delay(Delay).Wait();
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
            Print();
        }

        /// <summary>
        /// Resume the ProgresBar
        /// </summary>
        public void Resume()
        {
            ProgressStopwatch.Start();
            IsPaused = false;
            Print();
        }

        private void SetValue(int value)
        {
            if (value > Maximum)
                Maximum = value;
            _Value = value;
            TicksCompletedElements = value > 0 ? ProgressStopwatch.ElapsedTicks : (long?)null;
        }

        /// <summary>
        /// Advances the current position of the progress bar by the amount of the Step property
        /// </summary>
        /// <param name="newElementName">The name of the new Element</param>
        public void PerformStep(string newElementName = null)
        {
            if (newElementName != null)
                ElementName = newElementName;
            Value += Step;
        }

        public void WriteLine(ColorString colorString, bool truncateToOneLine = true)
        {
            var actions = colorString.GetConsoleWriteActions(s => Utils.AdaptTextToConsole(s, !truncateToOneLine) + Environment.NewLine);
            lock (ConsoleWriterLock)
            {
                ConsoleColor? oldForegroundColor = colorString.ForegroundColor.HasValue ? Console.ForegroundColor : (ConsoleColor?)null;
                ConsoleColor? oldBackgroundColor = colorString.BackgroundColor.HasValue ? Console.BackgroundColor : (ConsoleColor?)null; ;
                actions.ForEach(a => a.Invoke());
                if (oldForegroundColor.HasValue) Console.ForegroundColor = oldForegroundColor.Value;
                if (oldBackgroundColor.HasValue) Console.BackgroundColor = oldBackgroundColor.Value;
            }
            //If FixedInBottom and we written over the ProgressBar -> Print it again
            if (FixedInBottom && _NumberLastLinesWritten > 0 && Console.CursorTop >= _ConsoleRow)
                Print();
        }

        public void WriteLine(string value, bool truncateToOneLine = true)
            => WriteLine(new ColorString(value), truncateToOneLine);

        public void WriteLine(string value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null, bool truncateToOneLine = true)
            => WriteLine(new ColorString(value, foregroundColor, backgroundColor), truncateToOneLine);

        /// <summary>
        /// Prints in Console the ProgressBar
        /// </summary>
        public void Print()
        {
            List<Action> actionsProgressBar = GetConsoleActionsForProgressBarAndText();
            string emptyLine = Utils.GetEmptyConsoleLine();

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

                //Position
                if (FixedInBottom)
                {
                    if (_ConsoleRow < Console.WindowHeight - _NumberLastLinesWritten)
                    {
                        int newConsoleRow = Console.WindowHeight - _NumberLastLinesWritten;
                        if (_NumberLastLinesWritten > 0)
                        {
                            //Clear old ProgressBar
                            Console.SetCursorPosition(0, _ConsoleRow);
                            for (int i = _ConsoleRow; i < newConsoleRow; i++)
                                Console.WriteLine(emptyLine);
                        }
                        _ConsoleRow = newConsoleRow;
                    }
                    //if (oldCursorTop >= _ConsoleRow)
                    //{
                    //    //oldCursorTop is near or over: Keep 2 empty lines between Text and ProgressBar (avoid flickering)
                    //    Console.SetCursorPosition(0, oldCursorTop);
                    //    Console.WriteLine(emptyLine);
                    //    Console.WriteLine(emptyLine);
                    //    _ConsoleRow = oldCursorTop + 2;
                    //}

                    int scrollMargin = Math.Max((Console.WindowHeight - _NumberLastLinesWritten) / 3, 2);
                    if (_ConsoleRow - oldCursorTop <= scrollMargin / 2)
                    {
                        //oldCursorTop is near or over: Keep a margin between Text and ProgressBar (avoid flickering)
                        Console.SetCursorPosition(0, oldCursorTop);
                        for (int i = oldCursorTop; i < _ConsoleRow + _NumberLastLinesWritten; i++)
                            Console.WriteLine(emptyLine);
                        _ConsoleRow = oldCursorTop + scrollMargin;
                    }
                }
                else if (_ConsoleRow < 0)
                    _ConsoleRow = oldCursorTop;
                Console.SetCursorPosition(0, _ConsoleRow);

                //ProgressBar and Text
                actionsProgressBar.ForEach(a => a.Invoke());

                // Restore Cursor Position
                Console.SetCursorPosition(oldCursorLeft, oldCursorTop);

                //Show Cursor
                if (oldCursorVisible)
                    Console.CursorVisible = oldCursorVisible;

                //Restore colors
                Console.ForegroundColor = oldForegroundColor;
                Console.BackgroundColor = oldBackgroundColor;
            }
        }

        /// <summary>
        /// Unprints (remove) from Console last ProgressBar printed
        /// </summary>
        public void Unprint()
        {
            if (_ConsoleRow < 0 || _NumberLastLinesWritten <= 0)
                return;

            string emptyLine = Utils.GetEmptyConsoleLine();

            //Lock Write to console
            lock (ConsoleWriterLock)
            {
                int oldCursorLeft = Console.CursorLeft;
                int oldCursorTop = Console.CursorTop;
                bool oldCursorVisible = Console.CursorVisible;

                //Hide Cursor
                Console.CursorVisible = false;

                int initialRow = _ConsoleRow;
                if (FixedInBottom)
                    initialRow = Math.Max(_ConsoleRow, oldCursorTop);

                //Position
                Console.SetCursorPosition(0, initialRow);

                //Remove lines
                for (int i = initialRow; i < _ConsoleRow + _NumberLastLinesWritten; i++)
                    Console.WriteLine(emptyLine);

                // Restore Cursor Position
                Console.SetCursorPosition(oldCursorLeft, oldCursorTop);

                //Show Cursor
                if (oldCursorVisible)
                    Console.CursorVisible = oldCursorVisible;
            }
        }

        private List<Action> GetConsoleActionsForProgressBarAndText()
        {
            int oldLinesWrited = _NumberLastLinesWritten;
            string emptyLine = Utils.GetEmptyConsoleLine();

            // ProgressBar
            List<Action> list = GetConsoleActionsForProgressBar(true);

            // Text in same line
            ColorString coloredText = null;
            var maxTextLenght = Console.BufferWidth - Layout.TotalLength;
            if (maxTextLenght >= 10) //Text will be printed if there are 10 chars or more
            {
                if (IsDone)
                    coloredText = Layout.DoneTextGetter?.Invoke(this);
                else if (HasProgress)
                    coloredText = Layout.ProgressTextGetter?.Invoke(this);
                else
                    coloredText = Layout.NoProgressTextGetter?.Invoke(this);
                if (!string.IsNullOrEmpty(coloredText?.Value))
                    list.AddRange(coloredText.GetConsoleWriteActions(s => Utils.AdaptTextToMaxWidth(" " + s, maxTextLenght)));
            }
            list.Add(() => Console.Write(Environment.NewLine));
            _NumberLastLinesWritten = 1;

            // Description
            var descriptionLinesGetter = IsDone ? Layout.DoneDescriptionLinesGetter : Layout.DescriptionLinesGetter;
            if (descriptionLinesGetter != null)
            {
                foreach (var line in descriptionLinesGetter.Invoke(this))
                {
                    if (line != null)
                    {
                        int indentationLen = Layout.DescriptionLinesIndentation.Value?.Length ?? 0;
                        if (indentationLen > 0)
                            list.AddRange(Layout.DescriptionLinesIndentation.GetConsoleWriteActions());

                        list.AddRange(line.GetConsoleWriteActions(s => Utils.AdaptTextToMaxWidth(s, Console.BufferWidth - indentationLen)));
                        list.Add(() => Console.Write(Environment.NewLine));
                        _NumberLastLinesWritten++;
                    }
                }
            }

            // Clear old lines
            if (oldLinesWrited > _NumberLastLinesWritten)
            {
                for (int i = 0; i < oldLinesWrited - _NumberLastLinesWritten; i++)
                    list.Add(() => Console.WriteLine(emptyLine));
            }
            return list;
        }


        private void UpdateMarqueePosition()
        {
            int newProgressPosition = MarqueePosition + MarqueeIncrement;
            if (newProgressPosition < 0 || newProgressPosition >= Layout.InnerLength)
                MarqueeIncrement *= -1;

            MarqueePosition += MarqueeIncrement;
        }

        private List<Action> GetConsoleActionsForProgressBar(bool restoreColors)
        {
            var list = new List<Action>();

            ConsoleColor? oldForegroundColor = restoreColors ? Console.ForegroundColor : (ConsoleColor?)null;
            ConsoleColor? oldBackgroundColor = restoreColors ? Console.BackgroundColor : (ConsoleColor?)null;

            int progressLenght = HasProgress ? Convert.ToInt32(Percentage / (100f / Layout.InnerLength)) : 0;
            int pendingLenght = Layout.InnerLength - progressLenght;

            //Start
            list.AddRange(Layout.Start.GetConsoleWriteActions());

            //Body
            /*
             *      With Progress available (Maximum defined):
             *          [■■■■■■■■■■■■········] -> Without Marquee
             *          [■■■■■■■■■■■■····+···] -> With Marquee (in pending space) 
             *          [■■■■■■■■#■■■········] -> With Marquee (in progress space)
             *          
             *      Without Progress available (don't have Maximum):
             *          [·······■············] -> Marquee is always displayed
             */
            bool marqueeInProgress = HasProgress && ShowMarquee && MarqueePosition >= 0 && MarqueePosition < progressLenght;
            bool marqueeInPending = ShowMarquee && MarqueePosition >= progressLenght;

            int progressBeforeMarqueeLength = progressLenght;
            if (marqueeInProgress) progressBeforeMarqueeLength = MarqueePosition;

            int progressAfterMarqueeLength = 0;
            if (marqueeInProgress) progressAfterMarqueeLength = progressLenght - progressBeforeMarqueeLength - 1;

            int pendingBeforeMarqueeLength = pendingLenght;
            if (marqueeInPending) pendingBeforeMarqueeLength = MarqueePosition - progressLenght;

            int pendingAfterMarqueeLength = 0;
            if (marqueeInPending) pendingAfterMarqueeLength = pendingLenght - pendingBeforeMarqueeLength - 1;

            //Progress before Marquee
            list.AddRange(Layout.Progress.GetConsoleWriteActions(repetitions: progressBeforeMarqueeLength));

            //Marquee in progress
            if (marqueeInProgress)
                list.AddRange(Layout.MarqueeInProgress.GetConsoleWriteActions());

            //Progress after Marquee
            list.AddRange(Layout.Progress.GetConsoleWriteActions(repetitions: progressAfterMarqueeLength));

            //Pending before Marquee
            list.AddRange(Layout.Pending.GetConsoleWriteActions(repetitions: pendingBeforeMarqueeLength));

            //Marquee in Pending
            if (marqueeInPending)
            {
                if (HasProgress) list.AddRange(Layout.MarqueeInProgressPending.GetConsoleWriteActions());
                else list.AddRange(Layout.MarqueeAlone.GetConsoleWriteActions());
            }

            //Pending after Marquee
            list.AddRange(Layout.Pending.GetConsoleWriteActions(repetitions: pendingAfterMarqueeLength));

            //End
            list.AddRange(Layout.End.GetConsoleWriteActions());

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
            //UpdateRemainingTime();
            Print();
            if (FixedInBottom && _NumberLastLinesWritten > 0 && _ConsoleRow >= 0)
                Console.CursorTop = _ConsoleRow + _NumberLastLinesWritten;

            if (ProgressStopwatch.IsRunning)
                ProgressStopwatch.Stop();
            ProgressStopwatch.Reset();
        }
    }
}