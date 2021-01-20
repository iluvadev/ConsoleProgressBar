// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//
using ConsoleProgressBar.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgressBar
{
    /// <summary>
    /// A ProgressBar for Console
    /// </summary>
    public class ProgressBar : IDisposable
    {

        public class Element<T>
        {
            private Func<ProgressBar, T> _ValueGetter;
            private Func<ProgressBar, ConsoleColor> _ForegroundColorGetter;
            private Func<ProgressBar, ConsoleColor> _BackgroundColorGetter;
            private Func<ProgressBar, bool> _VisibleGetter;

            public Element<T> SetVisible(bool show)
                => SetVisible(pb => show);
            public Element<T> SetVisible(Func<ProgressBar, bool> showGetter)
            {
                _VisibleGetter = showGetter;
                return this;
            }
            public bool GetVisible(ProgressBar progressBar)
                => _VisibleGetter?.Invoke(progressBar) ?? true;

            public Element<T> SetValue(T value)
                  => SetValue(pb => value);
            public Element<T> SetValue(Func<ProgressBar, T> valueGetter)
            {
                _ValueGetter = valueGetter;
                return this;
            }
            public virtual T GetValue(ProgressBar progressBar)
                => GetVisible(progressBar) && _ValueGetter != null ? _ValueGetter.Invoke(progressBar) : default;

            public Element<T> SetForegroundColor(ConsoleColor foregroundColor)
                => SetForegroundColor(pb => foregroundColor);
            public Element<T> SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                _ForegroundColorGetter = foregroundColorGetter;
                return this;
            }
            public virtual ConsoleColor? GetForegroundColor(ProgressBar progressBar)
                => (GetVisible(progressBar) && _ForegroundColorGetter != null) ? _ForegroundColorGetter.Invoke(progressBar) : (ConsoleColor?)null;

            public Element<T> SetBackgroundColor(ConsoleColor backgroundColor)
                => SetBackgroundColor(pb => backgroundColor);

            public Element<T> SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                _BackgroundColorGetter = backgroundColorGetter;
                return this;
            }
            public virtual ConsoleColor? GetBackgroundColor(ProgressBar progressBar)
                => (GetVisible(progressBar) && _BackgroundColorGetter != null) ? _BackgroundColorGetter.Invoke(progressBar) : (ConsoleColor?)null;
        }

        public class ElementList<T>
        {
            public List<Element<T>> List { get; } = new List<Element<T>>();

            public ElementList<T> Clear()
            {
                List.Clear();
                return this;
            }

            public Element<T> AddNew()
            {
                var line = new Element<T>();
                List.Add(line);
                return line;
            }
        }

        /// <summary>
        /// Definition for the Marquee
        /// The Marquee is a char that moves around the ProgressBar
        /// </summary>
        public class LayoutMarqueeDefinition
        {
            /// <summary>
            /// Marquee definition when it moves over a 'Pending' step
            /// </summary>
            public Element<char> OverPending { get; } = new Element<char>();

            /// <summary>
            /// Marquee definition when it moves over a 'Progress' step
            /// </summary>
            public Element<char> OverProgress { get; } = new Element<char>();

            public LayoutMarqueeDefinition SetValue(char value)
                => SetValue(pb => value);
            public LayoutMarqueeDefinition SetValue(Func<ProgressBar, char> valueGetter)
            {
                OverPending.SetValue(valueGetter);
                OverProgress.SetValue(valueGetter);
                return this;
            }

            public LayoutMarqueeDefinition SetForegroundColor(ConsoleColor foregroundColor)
                => SetForegroundColor(pb => foregroundColor);
            public LayoutMarqueeDefinition SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                OverPending.SetForegroundColor(foregroundColorGetter);
                OverProgress.SetForegroundColor(foregroundColorGetter);
                return this;
            }

            public LayoutMarqueeDefinition SetBackgroundColor(ConsoleColor backgroundColor)
                => SetBackgroundColor(pb => backgroundColor);
            public LayoutMarqueeDefinition SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                OverPending.SetBackgroundColor(backgroundColorGetter);
                OverProgress.SetBackgroundColor(backgroundColorGetter);
                return this;
            }
            public LayoutMarqueeDefinition SetVisible(bool visible)
                => SetVisible(pb => visible);
            public LayoutMarqueeDefinition SetVisible(Func<ProgressBar, bool> showGetter)
            {
                OverPending.SetVisible(showGetter);
                OverProgress.SetVisible(showGetter);
                return this;
            }

            public LayoutMarqueeDefinition()
            {
                OverPending.SetValue(pb => pb.HasProgress ? '+' : '■')
                           .SetForegroundColor(pb => pb.HasProgress ? ConsoleColor.Yellow : ConsoleColor.Green);

                OverProgress.SetValue('■')
                            .SetForegroundColor(ConsoleColor.Yellow);
            }
        }

        public class LayoutMarginDefinition
        {
            public Element<string> Start { get; } = new Element<string>();
            public Element<string> End { get; } = new Element<string>();

            public LayoutMarginDefinition SetValue(string value)
                => SetValue(pb => value);
            public LayoutMarginDefinition SetValue(Func<ProgressBar, string> valueGetter)
            {
                Start.SetValue(valueGetter);
                End.SetValue(valueGetter);
                return this;
            }

            public LayoutMarginDefinition SetForegroundColor(ConsoleColor foregroundColor)
                => SetForegroundColor(pb => foregroundColor);
            public LayoutMarginDefinition SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                Start.SetForegroundColor(foregroundColorGetter);
                End.SetForegroundColor(foregroundColorGetter);
                return this;
            }

            public LayoutMarginDefinition SetBackgroundColor(ConsoleColor backgroundColor)
                => SetBackgroundColor(pb => backgroundColor);
            public LayoutMarginDefinition SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                Start.SetBackgroundColor(backgroundColorGetter);
                End.SetBackgroundColor(backgroundColorGetter);
                return this;
            }

            public LayoutMarginDefinition SetVisible(bool visible)
                => SetVisible(pb => visible);
            public LayoutMarginDefinition SetVisible(Func<ProgressBar, bool> showGetter)
            {
                Start.SetVisible(showGetter);
                End.SetVisible(showGetter);
                return this;
            }

            public int GetLength(ProgressBar progressBar)
                => (Start.GetValue(progressBar)?.Length ?? 0) + (End.GetValue(progressBar)?.Length ?? 0);

            public LayoutMarginDefinition()
            {
                Start.SetValue("[").SetForegroundColor(ConsoleColor.DarkBlue);
                End.SetValue("]").SetForegroundColor(ConsoleColor.DarkBlue);
            }
        }

        public class LayoutBodyDefinition
        {
            public Element<char> Pending { get; } = new Element<char>();
            public Element<char> Progress { get; } = new Element<char>();
            public Element<string> Text { get; } = new Element<string>();

            public LayoutBodyDefinition SetValue(char value)
                => SetValue(pb => value);
            public LayoutBodyDefinition SetValue(Func<ProgressBar, char> valueGetter)
            {
                Pending.SetValue(valueGetter);
                Progress.SetValue(valueGetter);
                return this;
            }

            public LayoutBodyDefinition SetForegroundColor(ConsoleColor foregroundColor)
                => SetForegroundColor(pb => foregroundColor);
            public LayoutBodyDefinition SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                Pending.SetForegroundColor(foregroundColorGetter);
                Progress.SetForegroundColor(foregroundColorGetter);
                return this;
            }

            public LayoutBodyDefinition SetBackgroundColor(ConsoleColor backgroundColor)
                => SetBackgroundColor(pb => backgroundColor);
            public LayoutBodyDefinition SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                Pending.SetBackgroundColor(backgroundColorGetter);
                Progress.SetBackgroundColor(backgroundColorGetter);
                return this;
            }

            public LayoutBodyDefinition()
            {
                Pending.SetValue('·').SetForegroundColor(ConsoleColor.DarkGray);
                Progress.SetValue('■').SetForegroundColor(ConsoleColor.DarkGreen);
                Text.SetVisible(false);
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

            public LayoutMarginDefinition Margins { get; } = new LayoutMarginDefinition();
            public LayoutMarqueeDefinition Marquee { get; } = new LayoutMarqueeDefinition();
            public LayoutBodyDefinition Body { get; } = new LayoutBodyDefinition();


            /// <summary>
            /// Width of entire ProgressBar
            /// Default = 30
            /// </summary>
            public int ProgressBarWidth { get; set; } = 30;

            /// <summary>
            /// Gets the internal Width of the ProgressBar
            /// </summary>
            public int GetInnerWidth(ProgressBar progressBar)
                => Math.Max(ProgressBarWidth - Margins.GetLength(progressBar), 0);

            public List<Action> GetRenderActions(ProgressBar progressBar)
            {
                var list = new List<Action>();

                //  [■■■■■■■■■■■■········] -> Without Marquee
                //  [■■■■■■■■■■■■····+···] -> Marquee over Pending space
                //  [■■■■■■■■#■■■········] -> Marquee over Progress space
                //  [·····+··············] -> Marquee withot progress

                int innerWidth = GetInnerWidth(progressBar);
                int progressLenght = progressBar.HasProgress ? Convert.ToInt32(progressBar.Percentage / (100f / innerWidth)) : 0;
                int pendingLenght = innerWidth - progressLenght;

                bool marqueeInProgress = progressBar.HasProgress &&
                    progressBar.MarqueePosition >= 0 && progressBar.MarqueePosition < progressLenght &&
                    Marquee.OverProgress.GetVisible(progressBar);
                bool marqueeInPending = progressBar.MarqueePosition >= progressLenght &&
                    Marquee.OverPending.GetVisible(progressBar);

                int progressBeforeMarqueeLength = progressLenght;
                if (marqueeInProgress) progressBeforeMarqueeLength = progressBar.MarqueePosition;

                int progressAfterMarqueeLength = 0;
                if (marqueeInProgress) progressAfterMarqueeLength = progressLenght - progressBeforeMarqueeLength - 1;

                int pendingBeforeMarqueeLength = pendingLenght;
                if (marqueeInPending) pendingBeforeMarqueeLength = progressBar.MarqueePosition - progressLenght;

                int pendingAfterMarqueeLength = 0;
                if (marqueeInPending) pendingAfterMarqueeLength = pendingLenght - pendingBeforeMarqueeLength - 1;

                string innerText = "";
                if (Body.Text.GetVisible(progressBar))
                    innerText = (Body.Text.GetValue(progressBar) ?? "").AdaptToMaxWidth(innerWidth);

                string textProgressBeforeMarquee = string.IsNullOrEmpty(innerText) ?
                                                   new string(Body.Progress.GetValue(progressBar), progressBeforeMarqueeLength)
                                                   : innerText.Substring(0, progressBeforeMarqueeLength);

                char? charProgressMarquee = null;
                if (marqueeInProgress) charProgressMarquee = string.IsNullOrEmpty(innerText) ?
                                                             Marquee.OverProgress.GetValue(progressBar)
                                                             : innerText[progressBar.MarqueePosition];

                string textProgressAfterMarquee = string.IsNullOrEmpty(innerText) ?
                                                  new string(Body.Progress.GetValue(progressBar), progressAfterMarqueeLength)
                                                  : innerText.Substring(progressBar.MarqueePosition + 1, progressAfterMarqueeLength);

                string textPendingBeforeMarquee = string.IsNullOrEmpty(innerText) ?
                                                  new string(Body.Pending.GetValue(progressBar), pendingBeforeMarqueeLength)
                                                  : innerText.Substring(progressLenght, pendingBeforeMarqueeLength);

                char? charPendingMarquee = null;
                if (marqueeInPending) charPendingMarquee = string.IsNullOrEmpty(innerText) ?
                                                           Marquee.OverPending.GetValue(progressBar)
                                                           : innerText[progressBar.MarqueePosition];

                string textPendingAfterMarquee = string.IsNullOrEmpty(innerText) ?
                                                 new string(Body.Pending.GetValue(progressBar), pendingAfterMarqueeLength)
                                                 : innerText.Substring(progressBar.MarqueePosition + 1, pendingAfterMarqueeLength);

                //Margin: Start
                list.AddRange(Margins.Start.GetRenderActions(progressBar));

                //Body: Progress before Marquee
                if (!string.IsNullOrEmpty(textProgressBeforeMarquee))
                {
                    var elementProgressBeforeMarquee = new Element<string>();
                    elementProgressBeforeMarquee.SetForegroundColor(Body.Progress.GetForegroundColor(progressBar) ?? Console.ForegroundColor);
                    elementProgressBeforeMarquee.SetBackgroundColor(Body.Progress.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                    elementProgressBeforeMarquee.SetValue(textProgressBeforeMarquee);
                    list.AddRange(elementProgressBeforeMarquee.GetRenderActions(progressBar));
                }

                //Body: Marquee in progress
                if (charProgressMarquee.HasValue)
                {
                    var elementProgressMarquee = new Element<char>();
                    elementProgressMarquee.SetForegroundColor(Marquee.OverProgress.GetForegroundColor(progressBar) ?? Console.ForegroundColor);
                    elementProgressMarquee.SetBackgroundColor(Marquee.OverProgress.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                    elementProgressMarquee.SetValue(charProgressMarquee.Value);
                    list.AddRange(elementProgressMarquee.GetRenderActions(progressBar));
                }

                //Body: Progress after Marquee
                if (!string.IsNullOrEmpty(textProgressAfterMarquee))
                {
                    var elementProgressAfterMarquee = new Element<string>();
                    elementProgressAfterMarquee.SetForegroundColor(Body.Progress.GetForegroundColor(progressBar) ?? Console.ForegroundColor);
                    elementProgressAfterMarquee.SetBackgroundColor(Body.Progress.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                    elementProgressAfterMarquee.SetValue(textProgressAfterMarquee);
                    list.AddRange(elementProgressAfterMarquee.GetRenderActions(progressBar));
                }

                //Body: Pending before Marquee
                if (!string.IsNullOrEmpty(textPendingBeforeMarquee))
                {
                    var elementPendingBeforeMarquee = new Element<string>();
                    elementPendingBeforeMarquee.SetForegroundColor(Body.Pending.GetForegroundColor(progressBar) ?? Console.ForegroundColor);
                    elementPendingBeforeMarquee.SetBackgroundColor(Body.Pending.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                    elementPendingBeforeMarquee.SetValue(textPendingBeforeMarquee);
                    list.AddRange(elementPendingBeforeMarquee.GetRenderActions(progressBar));
                }

                //Body: Marquee in Pending
                if (charPendingMarquee.HasValue)
                {
                    var elementPendingMarquee = new Element<char>();
                    elementPendingMarquee.SetForegroundColor(Marquee.OverPending.GetForegroundColor(progressBar) ?? Console.ForegroundColor);
                    elementPendingMarquee.SetBackgroundColor(Marquee.OverPending.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                    elementPendingMarquee.SetValue(charPendingMarquee.Value);
                    list.AddRange(elementPendingMarquee.GetRenderActions(progressBar));
                }

                //Body: Pending after Marquee
                if (!string.IsNullOrEmpty(textPendingAfterMarquee))
                {
                    var elementPendingAfterMarquee = new Element<string>();
                    elementPendingAfterMarquee.SetForegroundColor(Body.Pending.GetForegroundColor(progressBar) ?? Console.ForegroundColor);
                    elementPendingAfterMarquee.SetBackgroundColor(Body.Pending.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                    elementPendingAfterMarquee.SetValue(textPendingAfterMarquee);
                    list.AddRange(elementPendingAfterMarquee.GetRenderActions(progressBar));
                }

                //Margin: End
                list.AddRange(Margins.End.GetRenderActions(progressBar));

                return list;
            }
        }

        /// <summary>
        /// Definition for the Texts in a ProgressBar
        /// </summary>
        public class TextDefinition
        {
            public Element<string> Processing { get; } = new Element<string>();
            public Element<string> Paused { get; } = new Element<string>();
            public Element<string> Done { get; } = new Element<string>();

            public TextDefinition()
            {
                Processing.SetValue(pb =>
                    pb.HasProgress ?
                        $"{pb.Value} of {pb.Maximum} in {pb.TimeProcessing.ToStringWithAllHours()}, remaining: {pb.TimeRemaining.ToStringAsSumarizedRemainingText()}"
                        : $"Processing... ({pb.Value} in {pb.TimeProcessing.ToStringWithAllHours()})"
                        )
                    .SetForegroundColor(ConsoleColor.Cyan);

                Paused.SetValue(pb =>
                    pb.HasProgress ?
                        $"Paused... Running time: {pb.TimeProcessing.ToStringWithAllHours()}"
                        : $"{pb.Value} of {pb.Maximum} in {pb.TimeProcessing.ToStringWithAllHours()} (paused)"
                        )
                    .SetForegroundColor(ConsoleColor.DarkCyan);

                Done.SetValue("Done!")
                    .SetForegroundColor(ConsoleColor.DarkYellow);
            }

            public Element<string> GetCurrentText(ProgressBar progressBar)
            {
                if (progressBar == null)
                    return null;
                if (progressBar.IsPaused)
                    return Paused;
                if (progressBar.IsDone)
                    return Done;
                if (progressBar.IsStarted)
                    return Processing;
                return null;
            }
        }

        /// <summary>
        /// Definition for the Description lines in a ProgressBar
        /// </summary>
        public class DescriptionDefinition
        {
            public ElementList<string> Processing { get; } = new ElementList<string>();
            public ElementList<string> Paused { get; } = new ElementList<string>();
            public ElementList<string> Done { get; } = new ElementList<string>();

            public Element<string> Indentation { get; }
                 = new Element<string>();

            public DescriptionDefinition()
            {
                Processing.AddNew()
                    .SetValue(pb => pb.ElementName)
                    .SetVisible(pb => !string.IsNullOrEmpty(pb.ElementName))
                    .SetForegroundColor(ConsoleColor.DarkYellow);

                Paused.AddNew()
                    .SetValue("[Paused]")
                    .SetForegroundColor(ConsoleColor.DarkCyan);

                Done.AddNew()
                    .SetValue(pb => $"{pb.Value} in {pb.TimeProcessing.ToStringWithAllHours()} ({pb.TimePerElement.ToStringWithAllHours()} each one)")
                    .SetForegroundColor(ConsoleColor.DarkGray);

                Indentation.SetValue("  -> ").SetForegroundColor(ConsoleColor.DarkBlue);
            }

            public DescriptionDefinition Clear()
            {
                Processing.Clear();
                Paused.Clear();
                Done.Clear();
                return this;
            }

            public ElementList<string> GetCurrentDefinitionList(ProgressBar progressBar)
            {
                if (progressBar == null)
                    return null;
                if (progressBar.IsPaused)
                    return Paused;
                if (progressBar.IsDone)
                    return Done;
                if (progressBar.IsStarted)
                    return Processing;
                return null;
            }
        }

        private LayoutDefinition _Layout = null;
        /// <summary>
        /// Layout of the ProgressBar
        /// </summary>
        public LayoutDefinition Layout
        {
            get => _Layout ??= new LayoutDefinition();
            set => _Layout = value;
        }

        private TextDefinition _Text = null;
        /// <summary>
        /// Text of the ProgressBar
        /// </summary>
        public TextDefinition Text
        {
            get => _Text ??= new TextDefinition();
            set => _Text = value;
        }

        private DescriptionDefinition _Description = null;
        /// <summary>
        /// Description of the ProgressBar
        /// </summary>
        public DescriptionDefinition Description
        {
            get => _Description ??= new DescriptionDefinition();
            set => _Description = value;
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
            if (initialPosition.HasValue)
                _ConsoleRow = initialPosition.Value;
            ProgressStopwatch = new Stopwatch();

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
                                  IsStarted = true;
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

        /// <summary>
        /// Assigns the current Value, and optionally current ElementName and Tag
        /// If Value is greater than Maximum, updates Maximum as Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="elementName"></param>
        /// <param name="tag"></param>
        public void SetValue(int value, string elementName = null, object tag = null)
        {
            if (value > Maximum)
                Maximum = value;
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
        public void PerformStep(string elementName = null, object tag = null)
        {
            if (elementName != null) ElementName = elementName;
            if (tag != null) Tag = tag;

            Value += Step;
        }

        public void WriteLine()
            => WriteLine("", null, null, true);

        public void WriteLine(string value)
            => WriteLine(value, null, null, true);

        public void WriteLine(string value, bool truncateToOneLine)
            => WriteLine(value, null, null, truncateToOneLine);

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
                Print();
        }

        /// <summary>
        /// Prints in Console the ProgressBar
        /// </summary>
        public void Print()
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

                // Restore Cursor Position
                Console.SetCursorPosition(oldCursorLeft, oldCursorTop);

                //Show Cursor
                if (oldCursorVisible)
                    Console.CursorVisible = oldCursorVisible;
            }
        }

        private List<Action> GetRenderActionsForProgressBarAndText()
        {
            int oldLinesWrited = _NumberLastLinesWritten;
            string emptyLine = " ".AdaptToConsole();

            // ProgressBar
            List<Action> list = Layout.GetRenderActions(this);

            // Text in same line
            var maxTextLenght = Console.BufferWidth - Layout.ProgressBarWidth;
            if (maxTextLenght >= 10) //Text will be printed if there are 10 chars or more
            {
                var text = Text.GetCurrentText(this);
                if (text != null)
                    list.AddRange(text.GetRenderActions(this, s => (" " + s).AdaptToMaxWidth(maxTextLenght)));
            }
            list.Add(() => Console.Write(Environment.NewLine));
            _NumberLastLinesWritten = 1;

            // Description
            var descriptionList = Description.GetCurrentDefinitionList(this);
            if (descriptionList != null)
            {
                int indentationLen = Description.Indentation.GetValue(this)?.Length ?? 0;
                var maxDescLenght = Console.BufferWidth - indentationLen;
                foreach (var description in descriptionList.List.Where(d => d != null && d.GetVisible(this)))
                {
                    if (indentationLen > 0)
                        list.AddRange(Description.Indentation.GetRenderActions(this));

                    list.AddRange(description.GetRenderActions(this, s => s.AdaptToMaxWidth(maxDescLenght) + Environment.NewLine));
                    _NumberLastLinesWritten++;
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
            Print();
            if (FixedInBottom && _NumberLastLinesWritten > 0 && _ConsoleRow >= 0)
                Console.CursorTop = _ConsoleRow + _NumberLastLinesWritten;

            if (ProgressStopwatch.IsRunning)
                ProgressStopwatch.Stop();
            ProgressStopwatch.Reset();
        }
    }
}
namespace ConsoleProgressBar.Extensions
{
    public static class ElementExtensions
    {
        /// <summary>
        /// Returns a list of Actions to write the element in Console
        /// </summary>
        /// <param name="element"></param>
        /// <param name="progressBar"></param>
        /// <param name="valueTransformer">Function to Transform the value before write</param>
        /// <param name="repetitions">Number of repetitions</param>
        /// <returns></returns>
        public static List<Action> GetRenderActions(this ProgressBar.Element<string> element, ProgressBar progressBar, Func<string, string> valueTransformer = null)
        {
            var list = new List<Action>();

            if (progressBar == null || element == null || !element.GetVisible(progressBar))
                return list;

            var foregroundColor = element.GetForegroundColor(progressBar);
            if (foregroundColor.HasValue) list.Add(() => Console.ForegroundColor = foregroundColor.Value);

            var backgroundColor = element.GetBackgroundColor(progressBar);
            if (backgroundColor.HasValue) list.Add(() => Console.BackgroundColor = backgroundColor.Value);

            string value = element.GetValue(progressBar);
            if (valueTransformer != null) value = valueTransformer.Invoke(value);

            list.Add(() => Console.Write(value));
            list.Add(() => Console.ResetColor());

            return list;
        }

        public static List<Action> GetRenderActions(this ProgressBar.Element<char> element, ProgressBar progressBar, int repetition = 1)
        {
            var list = new List<Action>();

            if (progressBar == null || repetition < 1 || element == null || !element.GetVisible(progressBar))
                return list;

            var foregroundColor = element.GetForegroundColor(progressBar);
            if (foregroundColor.HasValue) list.Add(() => Console.ForegroundColor = foregroundColor.Value);

            var backgroundColor = element.GetBackgroundColor(progressBar);
            if (backgroundColor.HasValue) list.Add(() => Console.BackgroundColor = backgroundColor.Value);

            char value = element.GetValue(progressBar);
            list.Add(() => Console.Write(new string(value, repetition)));
            list.Add(() => Console.ResetColor());

            return list;
        }
    }


    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Gets a textual Sumarized for remaining time: X days, or Y hours, or Z minutes, etc.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string ToStringAsSumarizedRemainingText(this TimeSpan? ts)
            => ts.HasValue ? ToStringAsSumarizedRemainingText(ts.Value) : "unknown";

        /// <summary>
        /// Gets a textual Sumarized for remaining time: X days, or Y hours, or Z minutes, etc.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string ToStringAsSumarizedRemainingText(this TimeSpan ts)
        {
            int units;
            if ((units = Convert.ToInt32(Math.Round(ts.TotalDays))) > 1) return $"{units} days";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalDays))) == 1) return $"a day";
            else if ((units = Convert.ToInt32(Math.Round(ts.TotalHours))) > 1) return $"{units} hours";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalHours))) == 1) return $"an hour";
            else if ((units = Convert.ToInt32(Math.Round(ts.TotalMinutes))) > 1) return $"{units} minutes";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalMinutes))) == 1) return $"a minute";
            else if ((units = Convert.ToInt32(Math.Round(ts.TotalSeconds))) > 1) return $"{units} seconds";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalSeconds))) == 1) return $"a second";
            else return "a moment";
        }

        /// <summary>
        /// Converts a TimeSpan to String, showing all hours
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="includeMilliseconds"></param>
        /// <returns></returns>
        public static string ToStringWithAllHours(this TimeSpan? ts, bool includeMilliseconds = true)
            => ts.HasValue ? ToStringWithAllHours(ts.Value, includeMilliseconds) : "unknown";

        /// <summary>
        /// Converts a TimeSpan to String, showing all hours
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="includeMilliseconds"></param>
        /// <returns></returns>
        public static string ToStringWithAllHours(this TimeSpan ts, bool includeMilliseconds = true)
        {
            if (includeMilliseconds) return $"{ts.TotalHours:F0}{ts:\\:mm\\:ss\\.fff}";
            else return $"{ts.TotalHours:F0}{ts:\\:mm\\:ss}";
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Returns a string that occupy all console line/s
        /// </summary>
        /// <param name="value">The string to write in console</param>
        /// <param name="allowMultipleLines">To allow print the string in muliple lines or only in one:
        ///     True: The text can be represented in more than one Console line (fill spaces to the end of last line)
        ///     False: The text must be represented in only ONE line (truncate to fit or fill spaces to the end of line)
        /// </param>
        /// <returns></returns>
        public static string AdaptToConsole(this string value, bool allowMultipleLines = true)
        {
            int maxWidth = Console.BufferWidth;

            if (allowMultipleLines)
            {
                var lines = Math.DivRem(value.Length, maxWidth, out _) + 1;
                maxWidth *= lines;
            }
            return AdaptToMaxWidth(value, maxWidth);
        }

        /// <summary>
        /// Returns a string with exactly maxChars: Truncates string value or fill with spaces to fits exact length
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxWidth"></param>
        /// <param name="append">Text appended when it is truncated. Default: "..."</param>
        /// <returns></returns>
        public static string AdaptToMaxWidth(this string value, int maxWidth, string append = "...")
        {
            value = value ?? "";
            int len = value.Length;

            if (maxWidth <= 0) return "";
            else if (len == maxWidth) return value;
            else if (len < maxWidth) return value.PadRight(maxWidth);
            else return value.Substring(0, maxWidth - append.Length) + append;
        }
    }
}