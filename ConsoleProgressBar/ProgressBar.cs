// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//

using iluvadev.ConsoleProgressBar.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace iluvadev.ConsoleProgressBar
{
    /// <summary>
    /// A ProgressBar for Console
    /// </summary>
    public class ProgressBar : IDisposable
    {
        private Layout _Layout = null;
        /// <summary>
        /// Layout of the ProgressBar
        /// </summary>
        public Layout Layout
        {
            get => _Layout ??= new Layout();
            set => _Layout = value;
        }

        private Text _Text = null;
        /// <summary>
        /// Text definitions for the ProgressBar
        /// </summary>
        public Text Text
        {
            get=> _Text ??= new Text();
            set => _Text = value;
        }

        /// <summary>
        /// Tag object
        /// </summary>
        public object Tag { get; set; }

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
                    Unrender();
                    _FixedInBottom = value;
                    Render();
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

        internal int MarqueePosition { get; set; } = -1;
        private int MarqueeIncrement { get; set; } = 1;

        private bool CancelThread { get; set; }

        private Stopwatch ProgressStopwatch { get; set; }
        private long? TicksCompletedElements { get; set; }
        private long? TicksPerElement => TicksCompletedElements.HasValue && Value > 0 ? TicksCompletedElements.Value / Value : (long?)null;
        private long? TicksRemaining
        {
            get
            {
                if (!Maximum.HasValue || !TicksPerElement.HasValue || !TicksCompletedElements.HasValue) return (long?)null;
                long currentTicks = ProgressStopwatch.ElapsedTicks;
                long currentElementTicks = currentTicks - TicksCompletedElements.Value;
                long elementTicks = (currentElementTicks <= TicksPerElement.Value) ? TicksPerElement.Value : currentTicks / (Value + 1);
                long totalTicks = elementTicks * Maximum.Value;
                return Math.Max(totalTicks - currentTicks, 0);
            }
        }

        private int _ConsoleRow = -1;
        private int _NumberLastLinesWritten = -1;

        /// <summary>
        /// Creates an instance of ConsoleProgressBar
        /// </summary>
        /// <param name="initialPosition">Initial position of the ProgressBar</param>
        /// <param name="autoStart">True if ProgressBar starts automatically</param>
        public ProgressBar(int? initialPosition = null, bool autoStart = true)
        {
            ProgressStopwatch = new Stopwatch();
            if (initialPosition.HasValue) _ConsoleRow = initialPosition.Value;
            if (autoStart) Start();
        }

        /// <summary>
        /// Starts the ProgressBar
        /// </summary>
        public void Start()
        {
            if (IsStarted) Resume();
            else (new Thread(ThreadAction) { IsBackground = true }).Start();
        }

        private void ThreadAction()
        {
            ProgressStopwatch.Start();
            IsStarted = true;
            while (!CancelThread)
            {
                if (!IsPaused)
                {
                    try
                    {
                        UpdateMarqueePosition();
                        Render();
                        Task.Delay(Delay).Wait();
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Pauses the ProgressBar
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            ProgressStopwatch.Stop();
            Render();
        }

        /// <summary>
        /// Resume the ProgresBar
        /// </summary>
        public void Resume()
        {
            ProgressStopwatch.Start();
            IsPaused = false;
            Render();
        }

        /// <summary>
        /// Assigns the current Value, and optionally current ElementName and Tag
        /// If Value is greater than Maximum, updates Maximum as Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="elementName"></param>
        /// <param name="tag"></param>
        public void SetValue(int value, string elementName = null, object tag = null)
        {
            if (value > Maximum) Maximum = value;
            _Value = value;
            TicksCompletedElements = value > 0 ? ProgressStopwatch.ElapsedTicks : (long?)null;

            if (elementName != null) ElementName = elementName;
            if (tag != null) Tag = tag;
        }

        /// <summary>
        /// Advances the current position of the progress bar by the amount of the Step property
        /// </summary>
        /// <param name="elementName">The name of the new Element</param>
        /// <param name="tag"></param>
        public void PerformStep(string elementName = null, object tag = null) => PerformStep(Step, elementName, tag);

        /// <summary>
        /// Advances the current position of the progress bar by the amount of the Step property
        /// </summary>
        /// <param name="step">Step to perform</param>
        /// <param name="elementName">The name of the new Element</param>
        /// <param name="tag"></param>
        public void PerformStep(int step, string elementName = null, object tag = null)
        {
            if (elementName != null) ElementName = elementName;
            if (tag != null) Tag = tag;
            Value += step;
        }
        /// <summary>
        /// WriteLine in Console when ProgressBar is running
        /// </summary>
        public void WriteLine() => WriteLine("", null, null, true);
        /// <summary>
        /// WriteLine in Console when ProgressBar is running
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine(string value) => WriteLine(value, null, null, true);
        /// <summary>
        /// WriteLine in Console when ProgressBar is running
        /// </summary>
        /// <param name="value"></param>
        /// <param name="truncateToOneLine"></param>
        public void WriteLine(string value, bool truncateToOneLine) => WriteLine(value, null, null, truncateToOneLine);
        /// <summary>
        /// WriteLine in Console when ProgressBar is running
        /// </summary>
        /// <param name="value"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="truncateToOneLine"></param>
        public void WriteLine(string value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null, bool truncateToOneLine = true)
        {
            var actions = new List<Action>();

            if (foregroundColor.HasValue) actions.Add(() => Console.ForegroundColor = foregroundColor.Value);
            if (backgroundColor.HasValue) actions.Add(() => Console.BackgroundColor = backgroundColor.Value);
            actions.Add(() => Console.Write(value.AdaptToConsole(!truncateToOneLine) + Environment.NewLine));

            lock (ConsoleWriterLock)
            {
                if (foregroundColor.HasValue)
                {
                    var oldColor = Console.ForegroundColor;
                    actions.Add(() => Console.ForegroundColor = oldColor);
                }
                if (backgroundColor.HasValue)
                {
                    var oldColor = Console.BackgroundColor;
                    actions.Add(() => Console.BackgroundColor = oldColor);
                }
                actions.ForEach(a => a.Invoke());
            }

            //If FixedInBottom and we written over the ProgressBar -> Print it again
            if (FixedInBottom && _NumberLastLinesWritten > 0 && Console.CursorTop >= _ConsoleRow)
                Render();
        }

        /// <summary>
        /// Renders in Console the ProgressBar
        /// </summary>
        public void Render()
        {
            List<Action> actionsProgressBar = GetRenderActionsForProgressBarAndText();
            string emptyLine = " ".AdaptToConsole();

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
                        if (_NumberLastLinesWritten > 0 && _ConsoleRow >= 0)
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

                // Restore Cursor Position, Colors and Cursor visible
                Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
                Console.ForegroundColor = oldForegroundColor;
                Console.BackgroundColor = oldBackgroundColor;
                if (oldCursorVisible) Console.CursorVisible = oldCursorVisible;
            }
        }

        /// <summary>
        /// Unrenders (remove) from Console last ProgressBar printed
        /// </summary>
        public void Unrender()
        {
            if (_ConsoleRow < 0 || _NumberLastLinesWritten <= 0)
                return;

            string emptyLine = " ".AdaptToConsole();

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

                // Restore Cursor Position and Cursor Visible
                Console.SetCursorPosition(oldCursorLeft, oldCursorTop);
                if (oldCursorVisible) Console.CursorVisible = oldCursorVisible;
            }
        }

        private List<Action> GetRenderActionsForProgressBarAndText()
        {
            int oldLinesWritten = _NumberLastLinesWritten;
            string emptyLine = " ".AdaptToConsole();

            // ProgressBar
            List<Action> list = Layout.GetRenderActions(this);

            // Text in same line
            var maxTextLenght = Console.BufferWidth - Layout.ProgressBarWidth;
            if (maxTextLenght >= 10) //Text will be printed if there are 10 chars or more
            {
                var text = Text.Body.GetCurrentText(this);
                if (text != null)
                    list.AddRange(text.GetRenderActions(this, s => (" " + s).AdaptToMaxWidth(maxTextLenght)));
            }
            list.Add(() => Console.Write(Environment.NewLine));
            _NumberLastLinesWritten = 1;

            // Descriptions
            var descriptionList = Text.Description.GetCurrentDefinitionList(this)?.List?.Where(d => d != null && d.GetVisible(this));
            if (descriptionList != null && descriptionList.Any())
            {
                int indentationLen = Text.Description.Indentation.GetValue(this)?.Length ?? 0;
                var maxDescLenght = Console.BufferWidth - indentationLen;
                foreach (var description in descriptionList)
                {
                    if (indentationLen > 0) list.AddRange(Text.Description.Indentation.GetRenderActions(this));
                    list.AddRange(description.GetRenderActions(this, s => s.AdaptToMaxWidth(maxDescLenght) + Environment.NewLine));
                    _NumberLastLinesWritten++;
                }
            }

            // Clear old lines
            if (oldLinesWritten > _NumberLastLinesWritten)
            {
                for (int i = 0; i < oldLinesWritten - _NumberLastLinesWritten; i++)
                    list.Add(() => Console.WriteLine(emptyLine));
            }
            return list;
        }

        private void UpdateMarqueePosition()
        {
            int newProgressPosition = MarqueePosition + MarqueeIncrement;
            if (newProgressPosition < 0 || newProgressPosition >= Layout.GetInnerWidth(this))
                MarqueeIncrement *= -1;

            MarqueePosition += MarqueeIncrement;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CancelThread = true;
            Layout.Marquee.SetVisible(false);
            //UpdateRemainingTime();
            Render();
            if (FixedInBottom && _NumberLastLinesWritten > 0 && _ConsoleRow >= 0)
                Console.CursorTop = _ConsoleRow + _NumberLastLinesWritten;

            if (ProgressStopwatch.IsRunning)
                ProgressStopwatch.Stop();
            ProgressStopwatch.Reset();
        }
    }
}
