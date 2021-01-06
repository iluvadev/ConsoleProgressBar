using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProgressBar
{
    public class ProgressBarConsole : IDisposable
    {
        //Opcions
        public int InnerLength { get; set; } = 28;
        public int TotalLengt => InnerLength + Layout.Start.Length + Layout.End.Length;

        public ProgressBarLayout Layout { get; private set; }

        public int Maximum { get; set; } = 100;
        private int _Value = 0;
        public int Value
        {
            get => _Value;
            set => SetValue(value);
        }
        public int Step { get; set; } = 1;

        public bool ShowMarquee { get; set; } = true;
        public int MarqueeDelay { get; set; } = 75;
        public bool ShowProgress { get; set; } = true;

        public string CurrentElementName { get; set; }

        private int MarqueePosition { get; set; } = -1;
        private int MarqueeIncrement { get; set; } = 1;
        public int Percentage => Maximum != 0 ? ((Value * 100) / Maximum) : 100;

        private Stopwatch ProgressStopwatch { get; set; }
        public TimeSpan ProcessingTime => ProgressStopwatch.Elapsed;
        private TimeSpan TimePerElement => new TimeSpan(TicksPerElement);
        private long TicksPerElement
             => Value > 0 ? (long)Math.Round((decimal)ProgressStopwatch.ElapsedTicks / Value) : ProgressStopwatch.ElapsedTicks;
        public TimeSpan RemainingTime => new TimeSpan(TicksPerElement * (Maximum - Value));

        public bool IsDone => CancelThread || (ShowProgress && Value == Maximum);

        private int _ConsoleRow = -1;

        public Func<ProgressBarConsole, string> UndefinedProgressTextGetter { get; set; }
            = (pb) => $"Processing... ({pb.Value} in {pb.ProcessingTime.ToStringWithAllHours(true)})";

        public Func<ProgressBarConsole, string> ProgressTextGetter { get; set; }
            = (pb) => $"{pb.Value} of {pb.Maximum} in {pb.ProcessingTime.ToStringWithAllHours(true)}, remaining {pb.RemainingTime.ToStringWithAllHours()}";

        public Func<ProgressBarConsole, string[]> MultiLineGetter { get; set; }
            = (pb) => new string[] { $" > {pb.CurrentElementName}" };

        public Func<ProgressBarConsole, string> DoneTextGetter { get; set; }
            = (pb) => $"Done!";

        public Func<ProgressBarConsole, string[]> DoneMultiLineGetter { get; set; }
            = (pb) => new string[] { $" > {pb.Value} in {pb.ProcessingTime.ToStringWithAllHours(true)} ({pb.TimePerElement.ToStringWithAllHours(true)} each one)" };


        private Thread WorkingThread { get; set; }
        private bool CancelThread { get; set; }
        private bool Started { get; set; }
        private bool Paused { get; set; }
        private static readonly object ConsoleWriterLock = new object();

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
            if (Started)
                Resume();
            else
            {
                WorkingThread = new Thread(
                  () =>
                  {
                      ProgressStopwatch.Start();
                      while (!CancelThread)
                      {
                          if (!Paused)
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
            Paused = true;
            ProgressStopwatch.Stop();
        }
        public void Resume()
        {
            ProgressStopwatch.Start();
            Paused = false;
        }

        private void SetValue(int value)
        {
            if (value > Maximum)
                Maximum = value;
            _Value = value;
        }
        public void PerformStep(string newElementName = null)
        {
            CurrentElementName = newElementName;
            Value += Step;
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
            PrintProgressBar();

            if (ProgressStopwatch.IsRunning)
                ProgressStopwatch.Stop();
            ProgressStopwatch.Reset();
        }
    }
}