// Copyright (c) 2020, iluvadev, and released under MIT License.  This can be found in the root of this distribution. 

using ConsoleProgressBar;
using System;
using System.Threading.Tasks;

namespace ConsoleProgressBarDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Random random = new Random(Guid.NewGuid().GetHashCode());

            //Console.WriteLine("Demo for ProgressBar");
            //Console.WriteLine("---------------------------");

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBar with long running tasks:");
            //Console.WriteLine();
            //Console.ReadKey();
            //using (var pg = new ProgressBar() { Maximum = 5 })
            //{
            //    for (int i = 0; i < 5; i++)
            //    {
            //        var randomNum = random.Next(250);
            //        string elementName = "";
            //        for (int j = 0; j < randomNum; j++)
            //            elementName += (char)(random.Next(25) + 65);

            //        pg.CurrentElementName = elementName;
            //        Task.Delay(2000).Wait();
            //        pg.PerformStep();

            //    }
            //}
            //Console.ReadKey();

            //Console.ReadKey();
            using (var pg = new ProgressBar() { Maximum = 500,})
            {
                for (int i = 0; i < 500; i++)
                {
                    var randomNum = random.Next(150) + 1;
                    string elementName = "";
                    for (int j = 0; j < randomNum; j++)
                        elementName += (char)(random.Next(25) + 65);
                    if (i % 10 == 0)
                        Console.WriteLine(ProgressBar.AdaptTextToConsoleWidth($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: {i} - {elementName}"));
                    pg.CurrentElementName = elementName;
                    Task.Delay(200).Wait();
                    pg.PerformStep();

                }
            }
            //Console.ReadKey();

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBar without progress:");
            //Console.WriteLine();
            //Console.ReadKey();
            //using (var pg = new ProgressBar() { ShowProgress = false })
            //{
            //    for (int i = 0; i < 500; i++)
            //    {
            //        var randomNum = random.Next(100);
            //        string elementName = "";
            //        for (int j = 0; j < randomNum; j++)
            //            elementName += (char)(random.Next(25) + 97);

            //        pg.CurrentElementName = elementName;
            //        Task.Delay(10).Wait();
            //        pg.PerformStep();
            //    }
            //}
            //Console.ReadKey();



            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBarConsole without progress: ");
            ////Console.ReadKey();
            //using (var pg = new ProgressBarConsole { ShowProgress = false })
            //{
            //    pg.Text = "Doing something or waiting for a response...";
            //    Task.Delay(5000).Wait();
            //    pg.Text = "Ok, done.";
            //}
            ////Console.ReadKey();

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBarConsole without marquee: ");
            ////Console.ReadKey();
            //using (var pg = new ProgressBarConsole { ShowMarquee = false })
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        Task.Delay(100).Wait();
            //        pg.PerformStep();
            //    }
            //}
            ////Console.ReadKey();

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBarConsole with customization: ");
            ////Console.ReadKey();
            //ProgressBarLayout pgLayout1 = new ProgressBarLayout
            //{
            //    Start = "",
            //    End = "",
            //    Pending = '>',
            //    Progress = '█',
            //    MarqueeAlone = '·',
            //    MarqueeInProgressPending = '·',
            //    MarqueeInProgress = '▓',
            //};
            //using (var pg = new ProgressBarConsole(pgLayout1)
            //{
            //    ProgressTextFunc = null,
            //    InnerLength = Console.BufferWidth - pgLayout1.Start.Length - pgLayout1.End.Length,
            //    MarqueeDelay = 10,
            //    Maximum = 1000,
            //})
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Task.Delay(10).Wait();
            //        pg.PerformStep();
            //    }
            //}
            ////Console.ReadKey();

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBarConsole with customization: ");
            ////Console.ReadKey();
            //ProgressBarLayout pgLayout2 = new ProgressBarLayout
            //{
            //    Start = "",
            //    End = ">",
            //    Pending = ' ',
            //    Progress = '█',
            //    MarqueeAlone = '-',
            //    MarqueeInProgressPending = '·',
            //    MarqueeInProgress = '▓',
            //};
            //using (var pg = new ProgressBarConsole(pgLayout2)
            //{
            //    ProgressTextFunc = (pb) => pb.Text ?? $"Please, press any key. Time waiting: {pb.ProcessingTime.TotalSeconds}s",
            //    Value = 0,
            //    Maximum = 0,
            //    InnerLength = 5,
            //    MarqueeDelay = 100,
            //    ShowProgress = false
            //})
            //{
            //    var key = Console.ReadKey(true);
            //    pg.Text = $"Thanks! The key is {key.KeyChar}";
            //}
            ////Console.ReadKey();

            //Console.WriteLine("All done! Press enter to exit");
            //Console.ReadLine();
        }
    }
}
