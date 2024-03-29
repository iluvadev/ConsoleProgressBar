﻿// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//


using iluvadev.ConsoleProgressBar;
using iluvadev.ConsoleProgressBar.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iluvadev.ConsoleProgressBarDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();

            DemoProgressBar.Example11();
            //Example2();

            Console.ReadKey();
 

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
                    pb.Text.Description.Clear();
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
                    pb.Text.Body.Done
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
                    pb.Text.Description.Clear();

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

        private static void Example5()
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
            Console.Write("Dynamic layout:");
            Console.CursorVisible = false;
            Task taskPb1 = new Task(() =>
            {
                using (var pb = new ProgressBar(1, false) { Maximum = 100 })
                {

                    pb.Layout.Marquee.SetVisible(false);
                    pb.Text.Description.Clear();
                    pb.Text.Body.SetValue(pb => $"{pb.Percentage} %")
                           .SetForegroundColor(pb =>
                           {
                               if (pb.Percentage < 20) return ConsoleColor.DarkRed;
                               else if (pb.Percentage < 40) return ConsoleColor.Red;
                               else if (pb.Percentage < 60) return ConsoleColor.DarkYellow;
                               else if (pb.Percentage < 80) return ConsoleColor.DarkGreen;
                               return ConsoleColor.Green;
                           });

                    pb.Layout.Marquee.SetVisible(false);
                    pb.Layout.Margins.SetVisible(false);
                    pb.Layout.Body.SetForegroundColor(pb =>
                    {
                        if (pb.Percentage < 20) return ConsoleColor.DarkRed;
                        else if (pb.Percentage < 40) return ConsoleColor.Red;
                        else if (pb.Percentage < 60) return ConsoleColor.DarkYellow;
                        else if (pb.Percentage < 80) return ConsoleColor.DarkGreen;
                        return ConsoleColor.Green;
                    }).SetBackgroundColor(ConsoleColor.Black);
                    pb.Layout.Body.Pending.SetValue('─').SetForegroundColor(ConsoleColor.DarkGray);
                    pb.Layout.Body.Progress.SetValue('■');
                    pb.Layout.ProgressBarWidth = Console.BufferWidth / 2;

                    pb.Start();
                    for (int i = 0; i < 100; i++)
                    {
                        Task.Delay(random.Next(100)).Wait();
                        pb.PerformStep();
                    }
                }
            });
            taskPb1.Start();

            Console.SetCursorPosition(0, 4);
            Console.Write("Dynamic descriptions:");
            Console.CursorVisible = false;
            Task taskPb2 = new Task(() =>
            {
                using (var pb = new ProgressBar(5, false) { Maximum = 500 })
                {
                    pb.Text.Body.Done
                        .SetValue(p => $"Progress Bar 2: {p.Value} elements processed in {p.TimeProcessing.ToStringWithAllHours()}")
                        .SetForegroundColor(p => ConsoleColor.Green);

                    pb.Text.Description.Clear();

                    pb.Start();
                    for (int i = 0; i < 500; i++)
                    {
                        Task.Delay(10).Wait();
                        if ((i + 1) % 50 == 0)
                        {
                            var current = i + 1;
                            var currentProcessing = pb.TimeProcessing;
                            var currentRemaining = pb.TimeRemaining;
                            pb.Text.Description.Processing.AddNew().SetValue(pb => $"{current} elements processed in {currentProcessing.ToStringWithAllHours()}, remaining: {currentRemaining.ToStringWithAllHours()}; ({pb.TimeRemaining.ToStringWithAllHours()}) ");
                            //.SetForegroundColor(pb =>
                            //{
                            //    var dif = (pb.Value - current) % 78;
                            //    if (dif < 10) return ConsoleColor.White;
                            //    else if (dif < 20) return ConsoleColor.DarkBlue;
                            //    else if (dif < 30) return ConsoleColor.Yellow;
                            //    else if (dif < 40) return ConsoleColor.DarkGreen;
                            //    return ConsoleColor.Magenta;
                            //});
                            pb.Text.Description.Done.AddNew().SetValue($"{i + 1} elements processed in {pb.TimeProcessing.TotalSeconds} secs.");
                        }
                        pb.PerformStep(elementNames[i % elementNames.Count]);
                    }
                }
            });
            taskPb2.Start();



            Task.WaitAll(taskPb1, taskPb2);
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
