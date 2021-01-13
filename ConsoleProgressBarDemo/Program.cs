﻿// Copyright (c) 2020, iluvadev, and released under MIT License.  This can be found in the root of this distribution. 

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
                var randomNum = random.Next(200) + 20;
                string elementName = "";
                for (int j = 0; j < randomNum; j++)
                    elementName += (char)(random.Next(25) + 65);
                elementNames.Add(elementName);
            }

            //Console.WriteLine("Demo for ProgressBar");
            //Console.WriteLine("--------------------");
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(" ProgressBar with Progress and Marquee:");
            //Console.WriteLine();
            Console.ReadKey();

            Console.SetCursorPosition(0, 0);
            Console.Write("ProgressBar 1:");
            Task taskPb1 = new Task(() =>
            {
                using (var pb = new ProgressBar(1) { Maximum = 500, ShowMarquee = false })
                {
                    pb.Layout.DescriptionLinesGetter = null;
                    pb.Layout.DoneDescriptionLinesGetter = null;

                    pb.Layout.InnerLength = Console.BufferWidth;
                    pb.Layout.Start.Value = pb.Layout.End.Value = "";
                    pb.Layout.Pending.ForegroundColor = ConsoleColor.DarkRed;
                    pb.Layout.Progress.Value =
                    pb.Layout.Pending.Value =
                    pb.Layout.MarqueeInProgress.Value =
                    pb.Layout.MarqueeInProgressPending.Value =
                    pb.Layout.MarqueeAlone.Value = '─';

                    pb.Layout.DescriptionLinesIndentation.Value = "  └───> ";
                    pb.Layout.DoneDescriptionLinesGetter = (p => new List<ProgressBar.ColorString>
                    {
                        new ProgressBar.ColorString($"Progress Bar 1: {p.Value} done in {ProgressBar.Utils.ConvertToStringWithAllHours(p.TimeProcessing)}", ConsoleColor.DarkBlue)
                    });
                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(10).Wait();
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb1.Start();

            Console.SetCursorPosition(0, 4);
            Console.Write("ProgressBar 2:");
            Task taskPb2 = new Task(() =>
            {
                using (var pb = new ProgressBar(5) { Maximum = 500})
                {
                    pb.Layout.DescriptionLinesGetter = null;
                    pb.Layout.DoneDescriptionLinesGetter = null;
                    pb.Layout.DoneTextGetter = (p =>
                        new ProgressBar.ColorString($"Progress Bar 2: {p.Value} elements processed in {ProgressBar.Utils.ConvertToStringWithAllHours(p.TimeProcessing)}",
                                ConsoleColor.Green));
                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(15).Wait();
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb2.Start();

            Console.SetCursorPosition(0, 8);
            Console.Write("ProgressBar 3:");
            Task taskPb3 = new Task(() =>
            {
                using (var pb = new ProgressBar(9) { Maximum = 500})
                {
                    pb.Layout.InnerLength = Console.BufferWidth - 2;
                    pb.Layout.Pending.Value =
                    pb.Layout.MarqueeInProgressPending.Value = '─';

                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(20).Wait();
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb3.Start();

            Console.SetCursorPosition(0, 13);
            Console.Write("ProgressBar 4:");
            Task taskPb4 = new Task(() =>
            {
                using (var pb = new ProgressBar(14) { Maximum = null })
                {
                    pb.Layout.InnerLength = 15;
                    pb.Layout.Start.Value = pb.Layout.End.Value = "";
                    pb.Layout.Pending.Value = pb.Layout.MarqueeAlone.Value;
                    pb.Layout.Pending.ForegroundColor = ConsoleColor.DarkMagenta;
                    
                    pb.Layout.DescriptionLinesGetter = null;

                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(25).Wait();
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb4.Start();

            Task.WaitAll(taskPb1, taskPb2, taskPb3);


            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine(" ProgressBar with Progress and Marquee (default config)");
            Console.WriteLine();
            Console.ReadKey();
            using (var pb = new ProgressBar() { Maximum = 1000 })
            {
                for (int i = 0; i < 1000; i++)
                {
                    string elementName = elementNames[i % elementNames.Count];
                    Task.Delay(10).Wait();
                    pb.PerformStep(elementName);
                }
            }
            Console.ReadKey();

            //Console.WriteLine("àð═══i─");
            Console.WriteLine(" ProgressBar with KeepInLastLine (default) - Writing in Console");
            Console.WriteLine();
            Console.ReadKey();
            using (var pb = new ProgressBar() { Maximum = 500, FixedInBottom = true })
            {
                pb.Layout.DescriptionLinesGetter = null;
                for (int i = 0; i < 500; i++)
                {
                    string elementName = elementNames[i % elementNames.Count];
                    pb.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: Start processing {i} - {elementName}", false);
                    Task.Delay(10).Wait();
                    pb.PerformStep(elementName);
                    pb.WriteLine($"> [{DateTime.Now.ToString("HH:mm:ss.fff")}]: End processing {i} - {elementName}", false);
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
