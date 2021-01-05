using ConsoleProgressBar;
using System;
using System.Threading.Tasks;

namespace ConsoleProgressBarDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 0; i<10; i++)
            {
                Console.WriteLine($"Line {i}");
            }
            ProgressBarLayout pgLayout1 = new ProgressBarLayout();
            //{
            //    Start = '|',
            //    End = '|',
            //    Pending = '>',
            //    Progress = '█',
            //    MarqueeAlone = '·',
            //    MarqueeInProgressPending = '·',
            //    MarqueeInProgress = '▓',
            //};
            //using (var progressBar = new ProgressBarConsole(pgLayout1) { ShowProgress = false })
            //{
            //    Task.Delay(3000).Wait();
            //}
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Line {i}");
            }
            //using (var pb = new ProgressBarConsole(pgLayout1) { InnerLength = 30, Step = 2, ShowProgress = true, ShowMarquee = true })
            //{
            //    for (int i = 0; i < 50; i++)
            //    {
            //        Task.Delay(300).Wait();
            //        pb.PerformStep($"Processed i={i}, item {pb.Value} of {pb.Maximum} ({pb.Percentage}%)");
            //    }
            //    pb.Text = "Done!";
            //}
            using (var progressBar = new ProgressBarConsole(pgLayout1))
            {
                progressBar.Maximum = 1000;
                progressBar.ShowMarquee = true;
                progressBar.InnerLength = 20;
                for (int i = 0; i < 1000; i++)
                {
                    progressBar.PerformStep();
                    Task.Delay(10).Wait();
                }
            }
        }
    }
}
