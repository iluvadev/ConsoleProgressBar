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

        public string Text { get; set; }

        private int MarqueePosition { get; set; } = -1;
        private int MarqueeIncrement { get; set; } = 1;
        public int Percentage => Maximum != 0 ? ((Value * 100) / Maximum) : 100;
        //private string LastProgressRemoval { get; set; }

        private Stopwatch ProgressStopwatch { get; set; }
        public TimeSpan ProcessingTime => ProgressStopwatch.Elapsed;
        public TimeSpan RemainingTime { get; private set; }

        public bool IsDone => Value == Maximum;

        public Func<ProgressBarConsole, string> ProgressTextFunc { get; set; }
            = (pb) =>
            {
                if (!string.IsNullOrEmpty(pb.Text))
                    return pb.Text;
                else if (pb.IsDone)
                    return $"Done! {pb.Maximum} in {pb.ProcessingTime.TotalHours:F0}{pb.ProcessingTime:\\:mm\\:ss\\.fff}";
                else if (pb.ShowProgress)
                    return $"{pb.Value} of {pb.Maximum} in {pb.ProcessingTime.TotalHours:F0}{pb.ProcessingTime:\\:mm\\:ss\\.fff}, remaining {pb.RemainingTime.TotalHours:F0}{pb.RemainingTime:\\:mm\\:ss}";
                return null;
            };

        private Thread WorkingThread { get; set; }
        private bool CancelThread { get; set; }
        private bool Started { get; set; }
        private bool Paused { get; set; }

        public ProgressBarConsole(bool autoStart, ProgressBarLayout layout)
        {
            ProgressStopwatch = new Stopwatch();
            RemainingTime = new TimeSpan(0);
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

            if (value != 0)
                UpdateReminingTime();
        }
        public void PerformStep(string text = null)
        {
            Text = text;
            Value += Step;
        }

        //private void RemoveLastProgressBar()
        //{
        //    if (!string.IsNullOrEmpty(LastProgressRemoval))
        //        Console.Write(LastProgressRemoval);
        //}

        private void PrintProgressBar()
        {
            //RemoveLastProgressBar();

            string progress = GetProgressBar();
            string text = ProgressTextFunc?.Invoke(this) ?? Text;
            if (!string.IsNullOrEmpty(text))
                progress += " " + text;


            Console.Write("\r" + progress + new string(' ', Console.BufferWidth - progress.Length));

            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < progress.Length; i++)
            //    sb.Append("\b \b");

            //LastProgressRemoval = sb.ToString();
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

        private void UpdateReminingTime()
        {
            long ticksPerItem = (long)Math.Round((decimal)ProgressStopwatch.ElapsedTicks / Value);
            TimeSpan newRemainingTime = new TimeSpan(ticksPerItem * (Maximum - Value));
            if (RemainingTime.TotalSeconds == 0)
                RemainingTime = newRemainingTime;
            else
            {
                var difSeconds = (int)Math.Abs((newRemainingTime - RemainingTime).TotalSeconds);
                if (difSeconds >= 1)
                {
                    if (newRemainingTime > RemainingTime)
                        RemainingTime = RemainingTime.Add(new TimeSpan(0, 0, difSeconds));
                    else
                        RemainingTime = RemainingTime.Add(new TimeSpan(0, 0, (difSeconds / 2) * -1));
                }
            }
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

            Console.WriteLine(string.Empty);
        }
    }
}