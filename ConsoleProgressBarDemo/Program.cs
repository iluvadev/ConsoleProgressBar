// Copyright (c) 2020, iluvadev, and released under MIT License.  This can be found in the root of this distribution. 

using ConsoleProgressBar;
using ConsoleProgressBar.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleProgressBarDemo
{
    class Program
    {
        static void Main(string[] args)
        {


            //Example2();
            //Example3();
            Example4();

            Console.ReadKey();
            Console.CursorVisible = false;
            Example_Usage1();
            Console.ReadKey();


            //Console.WriteLine();
            //Console.WriteLine(" ProgressBar with Progress and Marquee (default config)");
            //Console.WriteLine();
            //Console.ReadKey();
            //using (var pb = new ProgressBar() { Maximum = 1000 })
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        string elementName = elementNames[i % elementNames.Count];
            //        Task.Delay(10).Wait();
            //        pb.PerformStep(elementName);
            //    }
            //}
            //Console.ReadKey();

            ////Console.WriteLine("àð═══i─");
            //Console.WriteLine(" ProgressBar with KeepInLastLine (default) - Writing in Console");
            //Console.WriteLine();
            //Console.ReadKey();
            //using (var pb = new ProgressBar() { Maximum = 500, FixedInBottom = true })
            //{
            //    pb.Description.Clear();

            //    for (int i = 0; i < 500; i++)
            //    {
            //        string elementName = elementNames[i % elementNames.Count];
            //        pb.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]: Start processing {i} - {elementName}", false);
            //        Task.Delay(10).Wait();
            //        pb.PerformStep(elementName);
            //        pb.WriteLine($"> [{DateTime.Now.ToString("HH:mm:ss.fff")}]: End processing {i} - {elementName}", false);
            //    }
            //}
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

        private static void Example1()
        {
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

            Console.ReadKey();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write("ProgressBar 1:");
            Console.CursorVisible = false;
            Task taskPb1 = new Task(() =>
            {
                using (var pb = new ProgressBar(1, false) { Maximum = 500 })
                {
                    pb.Description.Clear();
                    pb.Description.Done
                        .AddNew()
                        .SetValue(p => $"Progress Bar 1: {p.Value} done in {p.TimeProcessing.ToStringWithAllHours()}")
                        .SetForegroundColor(ConsoleColor.DarkBlue);
                    pb.Description.Indentation.SetValue("  └───> ");

                    pb.Layout.Marquee.SetVisible(false);
                    pb.Layout.Margins.SetVisible(false);
                    pb.Layout.Body.SetValue('─').Pending.SetForegroundColor(ConsoleColor.DarkRed);
                    pb.Layout.Marquee.SetValue('─');
                    pb.Layout.ProgressBarWidth = Console.BufferWidth;

                    pb.Start();
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
            Console.CursorVisible = false;
            Task taskPb2 = new Task(() =>
            {
                using (var pb = new ProgressBar(5, false) { Maximum = 500 })
                {
                    pb.Text.Done
                        .SetValue(p => $"Progress Bar 2: {p.Value} elements processed in {p.TimeProcessing.ToStringWithAllHours()}")
                        .SetForegroundColor(p => ConsoleColor.Green);

                    pb.Description.Clear();

                    pb.Start();
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
            Console.CursorVisible = false;
            Task taskPb3 = new Task(() =>
            {
                using (var pb = new ProgressBar(9, false) { Maximum = 500 })
                {
                    pb.Layout.ProgressBarWidth = Console.BufferWidth;
                    pb.Layout.Body.Pending.SetValue('─');
                    pb.Layout.Marquee.OverPending.SetValue('─');

                    pb.Start();
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
            Console.CursorVisible = false;
            Task taskPb4 = new Task(() =>
            {
                using (var pb = new ProgressBar(14, false) { Maximum = null })
                {
                    pb.Layout.ProgressBarWidth = 15;
                    pb.Layout.Margins.SetVisible(false);
                    pb.Layout.Body.Pending.SetValue('■').SetForegroundColor(ConsoleColor.Magenta);
                    pb.Layout.Marquee.OverPending.SetValue('■');

                    pb.Description.Clear();
                    pb.Description.Processing.AddNew().SetValue(p => p.Value.ToString()).SetForegroundColor(ConsoleColor.Yellow);
                    pb.Description.Processing.AddNew().SetValue(p => p.TimeProcessing.ToStringWithAllHours()).SetForegroundColor(ConsoleColor.Cyan);

                    pb.Start();
                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(25).Wait();
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb4.Start();

            Task.WaitAll(taskPb1, taskPb2, taskPb3, taskPb4);
        }
        private static void Example2()
        {
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

            Console.ReadKey();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write("ProgressBar 5:");
            Console.CursorVisible = false;
            Task taskPb1 = new Task(() =>
            {
                using (var pb = new ProgressBar(1, false) { Maximum = 500 })
                {
                    pb.Description.Clear();
                    pb.Layout.Marquee.SetVisible(false);
                    pb.Layout.Margins.SetVisible(false);
                    pb.Layout.Body.Pending.SetForegroundColor(ConsoleColor.White).SetBackgroundColor(ConsoleColor.DarkRed);
                    pb.Layout.Body.Progress.SetForegroundColor(ConsoleColor.Black).SetBackgroundColor(ConsoleColor.DarkGreen);
                    pb.Layout.ProgressBarWidth = Console.BufferWidth;

                    pb.Layout.Body.Text.SetVisible(true).SetValue(p =>
                    {
                        if (p.IsDone)
                            return $"{p.Value} elements processed in {p.TimeProcessing.ToStringWithAllHours()}";
                        else
                            return $"{p.Percentage}%... Remaining: {p.TimeRemaining.ToStringWithAllHours(false)} - Current: {p.ElementName}";
                    });

                    pb.Start();
                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(10).Wait();
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb1.Start();

            Console.SetCursorPosition(0, 4);
            Console.Write("ProgressBar 6:");
            Console.CursorVisible = false;
            Task taskPb2 = new Task(() =>
            {
                using (var pb = new ProgressBar(5, false) { Maximum = 500 })
                {
                    pb.Text.Done
                        .SetValue(p => $"Progress Bar 6: {p.Value} elements processed in {p.TimeProcessing.ToStringWithAllHours()}")
                        .SetForegroundColor(p => ConsoleColor.Green);

                    string text = "------Processing------";
                    string textDone = "---------Done---------";
                    pb.Layout.Margins.SetVisible(false);
                    pb.Layout.ProgressBarWidth = text.Length + pb.Layout.Margins.GetLength(pb);
                    pb.Layout.Body.Text.SetVisible(true).SetValue(p => p.IsDone ? textDone : text);
                    pb.Layout.Body.Progress.SetForegroundColor(ConsoleColor.Green);
                    pb.Layout.Body.Pending.SetForegroundColor(ConsoleColor.DarkRed);
                    pb.Layout.Body.SetBackgroundColor(ConsoleColor.Black);
                    pb.Layout.Marquee.SetBackgroundColor(ConsoleColor.Black);
                    pb.Description.Clear();

                    pb.Start();
                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(15).Wait();
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb2.Start();

            //Console.SetCursorPosition(0, 8);
            //Console.Write("ProgressBar 3:");
            //Console.CursorVisible = false;
            //Task taskPb3 = new Task(() =>
            //{
            //    using (var pb = new ProgressBar(9, false) { Maximum = 500 })
            //    {
            //        pb.Layout.ProgressBarWidth = Console.BufferWidth;
            //        pb.Layout.Body.Pending.SetValue('─');
            //        pb.Layout.Marquee.OverPending.SetValue('─');

            //        pb.Start();
            //        for (int i = 0; i < 500; i++)
            //        {
            //            Task.Delay(20).Wait();
            //            pb.PerformStep(elementNames[i % elementNames.Count]);
            //        }
            //    }
            //});
            //taskPb3.Start();

            //Console.SetCursorPosition(0, 13);
            //Console.Write("ProgressBar 4:");
            //Console.CursorVisible = false;
            //Task taskPb4 = new Task(() =>
            //{
            //    using (var pb = new ProgressBar(14, false) { Maximum = null })
            //    {
            //        pb.Layout.ProgressBarWidth = 15;
            //        pb.Layout.Margins.SetVisible(false);
            //        pb.Layout.Body.Pending.SetValue('■').SetForegroundColor(ConsoleColor.Magenta);
            //        pb.Layout.Marquee.OverPending.SetValue('■');

            //        pb.Description.Clear();
            //        pb.Description.Processing.AddNew().SetValue(p => p.Value.ToString()).SetForegroundColor(ConsoleColor.Yellow);
            //        pb.Description.Processing.AddNew().SetValue(p => p.TimeProcessing.ToStringWithAllHours()).SetForegroundColor(ConsoleColor.Cyan);

            //        pb.Start();
            //        for (int i = 0; i < 500; i++)
            //        {
            //            Task.Delay(25).Wait();
            //            pb.PerformStep(elementNames[i % elementNames.Count]);
            //        }
            //    }
            //});
            //taskPb4.Start();

            Task.WaitAll(taskPb1, taskPb2);
        }

        private static void Example3()
        {
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

            Console.ReadKey();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write("ProgressBar 7:");
            Console.CursorVisible = false;

            using (var pb = new ProgressBar() { Maximum = 500 })
            {
                pb.WriteLine();
                pb.WriteLine();
                for (int i = 0; i < 500; i++)
                {
                    Task.Delay(10).Wait();
                    pb.PerformStep(elementNames[i % elementNames.Count]);
                    if ((i + 1) % 100 == 0)
                        pb.WriteLine($"We can write... Element {i + 1} processed");
                }
            }
        }

        private static void Example4()
        {
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

            Console.ReadKey();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("ProgressBar 8:");
            Console.WriteLine("=============");
            Console.WriteLine();
            Console.CursorVisible = false;

            using (var pb = new ProgressBar(autoStart: false) { Maximum = 70 })
            {
                pb.FixedInBottom = true;
                pb.Start();
                for (int i = 0; i < 70; i++)
                {
                    Task.Delay(120).Wait();
                    string elementName = elementNames[i % elementNames.Count].ToLowerInvariant();
                    pb.PerformStep(elementName);
                    pb.WriteLine($"[Processed at {pb.TimeProcessing.ToStringWithAllHours(true)}] '{elementName}'", false);
                }
            }
        }

        private static void Example_Usage1()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar { Maximum = max })
            {
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(50).Wait(); //Do thinks
                    pb.PerformStep(); //Step in ProgressBar
                }
            }
        }
    }
}
