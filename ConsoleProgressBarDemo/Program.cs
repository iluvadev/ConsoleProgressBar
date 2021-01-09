// Copyright (c) 2020, iluvadev, and released under MIT License.  This can be found in the root of this distribution. 

using ConsoleProgressBar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleProgressBarDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            //Randomize elementNames
            var elementNames = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                var randomNum = random.Next(200)+20;
                string elementName = "";
                for (int j = 0; j < randomNum; j++)
                    elementName += (char)(random.Next(25) + 65);
                elementNames.Add(elementName);
            }

            //Console.WriteLine("Demo for ProgressBar");
            //Console.WriteLine("---------------------------");
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBar with Progress and Marquee:");
            //Console.WriteLine();
            //Console.ReadKey();
            //using (var pg = new ProgressBar() { Maximum = 1000 })
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        pg.ElementName = elementNames[i % elementNames.Count];
            //        Task.Delay(10).Wait();
            //        pg.PerformStep();

            //    }
            //}
            //Console.ReadKey();

            Console.WriteLine();
            Console.WriteLine(" ProgressBar with KeepInLastLine (default) - Writing in Console");
            Console.WriteLine();
            Console.ReadKey();
            using (var pg = new ProgressBar() { Maximum = 500, FixedInBottom = false })
            {
                for (int i = 0; i < 500; i++)
                {
                    string elementName = elementNames[i % elementNames.Count];
                    //pg.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: {i} - {elementName}", false);

                    pg.Layout.DescriptionLinesGetter = null;
                    pg.ElementName = elementName;
                    Task.Delay(10).Wait();
                    pg.PerformStep();
                    if ((i+1) % 100 == 0)
                        pg.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: {i+1} - in {ProgressBar.Utils.ConvertToStringWithAllHours(pg.TimeProcessing, false)}");
                }
            }
            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine(" ProgressBar with KeepInLastLine (default) - Writing in Console");
            Console.WriteLine();
            Console.ReadKey();
            using (var pg = new ProgressBar() { Maximum = 500, FixedInBottom = true })
            {
                for (int i = 0; i < 500; i++)
                {
                    string elementName = elementNames[i % elementNames.Count];
                    pg.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: {i} - {elementName}", false);

                    pg.Layout.DescriptionLinesGetter = null;
                    pg.ElementName = elementName;
                    Task.Delay(10).Wait();
                    pg.PerformStep();
                    if ((i + 1) % 100 == 0)
                        pg.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: {i + 1} - in {ProgressBar.Utils.ConvertToStringWithAllHours(pg.TimeProcessing, false)}");
                }
            }
            Console.ReadKey();

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
