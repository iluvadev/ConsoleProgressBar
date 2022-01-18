using iluvadev.ConsoleProgressBar;
using System;
using System.Threading.Tasks;

namespace iluvadev.ConsoleProgressBarDemo
{
    internal class DemoProgressBar
    {
        public static void Example01()
        {
            const int max = 500;

            //Create the ProgressBar
            // Maximum: The Max value in ProgressBar (Default is 100)
            using (var pb = new ProgressBar { Maximum = max })
            {
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
        }

        public static void Example02()
        {
            const int max = 1000;

            //Create the ProgressBar
            // initialPosition: Console Line to put the ProgressBar (optional, last Console line is assumed)
            // autoStart: Optional, default true
            // Maximum: The Max value in ProgressBar (Default is 100)
            // Step: The increment when performStep (Default is 1)
            using (var pb = new ProgressBar(initialPosition: 3, autoStart: false) { Maximum = max, Step = 2 })
            {
                pb.Start(); // autoStart=false -> we need start manually
                for (int i = 0; i < max; i += pb.Step)
                {
                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar
                }
            }
        }

        public static void Example03()
        {
            const int max = 500;

            //Create the ProgressBar
            // Maximum: The Max value in ProgressBar (Default is 100)
            using (var pb = new ProgressBar() { Maximum = null })
            {
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
        }

        public static void Example04()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar() { Maximum = max })
            {
                //Set "Processing Text" Visible = false
                pb.Text.Body.Processing.SetVisible(false);

                for (int i = 0; i < max; i++)
                {
                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
        }
        public static void Example05()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar() { Maximum = max })
            {
                //Setting fixed "Processing Text" 
                pb.Text.Body.Processing.SetValue("Processing, please wait...");

                //Setting "Done Text"
                pb.Text.Body.Done.SetValue("Well done!!");

                //Clear "Description Text"
                pb.Text.Description.Clear();

                for (int i = 0; i < max; i++)
                {
                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
        }

        public static void Example06()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar() { Maximum = max })
            {
                //Setting "Processing Text" with context 
                pb.Text.Body.Processing.SetValue(pb => ($"Processing {pb.ElementName}, please wait..."));

                //Setting "Done Text" with context
                pb.Text.Body.Done.SetValue(pb => $"Processed {pb.Maximum} in {pb.TimeProcessing.TotalSeconds}s.");

                //Clear "Description Text"
                pb.Text.Description.Clear();

                for (int i = 0; i < max; i++)
                {
                    string elementName = Guid.NewGuid().ToString();

                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
                }
            }
        }
        public static void Example07()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar() { Maximum = max })
            {
                //Clear "Description Text"
                pb.Text.Description.Clear();

                //Setting "Description Text" when "Processing"
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Element: {pb.ElementName}");
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Count: {pb.Value}");
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Processing time: {pb.TimeProcessing.TotalSeconds}s.");
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"Estimated remaining time: {pb.TimeRemaining?.TotalSeconds}s.");

                //Setting "Description Text" when "Done"
                pb.Text.Description.Done.AddNew().SetValue(pb => $"{pb.Value} elements in {pb.TimeProcessing.TotalSeconds}s.");

                for (int i = 0; i < max; i++)
                {
                    string elementName = Guid.NewGuid().ToString();

                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
                }
            }
        }

        public static void Example08()
        {
            const int max = 150;

            //Create the ProgressBar
            using (var pb = new ProgressBar() { Maximum = max, FixedInBottom = true })
            {
                //Clear "Description Text"
                pb.Text.Description.Clear();

                for (int i = 0; i < max; i++)
                {
                    string elementName = Guid.NewGuid().ToString();

                    Task.Delay(50).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)

                    //Writing on Console with ProgressBar
                    pb.WriteLine($"> [{DateTime.Now:HH:mm:ss.fff}]: Processed {i}: {elementName}");
                }
            }
        }

        public static void Example09()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar(autoStart: false) { Maximum = max })
            {
                // Hide Text
                pb.Text.Body.SetVisible(false);
                
                // Clear "Description Text"
                pb.Text.Description.Clear();

                // Setting "Description Indentation", with color
                pb.Text.Description.Indentation.SetValue("└───> ")
                                               .SetForegroundColor(ConsoleColor.Cyan);

                // Setting "Description Text" when "Done", with color
                pb.Text.Description.Done.AddNew().SetValue(pb => $"{pb.Value} elements in {pb.TimeProcessing.TotalSeconds}s.")
                                                 .SetForegroundColor(ConsoleColor.DarkBlue);
                
                // Hide "Margins"
                pb.Layout.Margins.SetVisible(false);

                // Setting "Body" layout
                pb.Layout.Body.SetValue('─');
                pb.Layout.Body.Progress.SetForegroundColor(ConsoleColor.DarkGreen);
                pb.Layout.Body.Pending.SetForegroundColor(ConsoleColor.DarkRed);
                
                // Setting "Marquee" layout
                pb.Layout.Marquee.SetValue('─');
                pb.Layout.Marquee.OverProgress.SetForegroundColor(ConsoleColor.Yellow);
                pb.Layout.Marquee.OverPending.SetForegroundColor(ConsoleColor.DarkYellow);

                // Setting ProgressBar width
                pb.Layout.ProgressBarWidth = Console.BufferWidth;

                pb.Start();

                for (int i = 0; i < max; i++)
                {
                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
        }

        public static void Example10()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar(autoStart: false) { Maximum = max })
            {
                // Hide Text
                pb.Text.Body.SetVisible(false);

                // Clear "Description Text"
                pb.Text.Description.Clear();

                // Hide "Margins"
                pb.Layout.Margins.SetVisible(false);

                // Hide "Marquee"
                pb.Layout.Marquee.SetVisible(false);

                // Setting Body Colors
                pb.Layout.Body.Pending.SetForegroundColor(ConsoleColor.White).SetBackgroundColor(ConsoleColor.DarkRed);
                pb.Layout.Body.Progress.SetForegroundColor(ConsoleColor.Black).SetBackgroundColor(ConsoleColor.DarkGreen);

                // Setting Body Text (internal text), from Layout
                pb.Layout.Body.Text.SetVisible(true).SetValue(pb =>
                {
                    if (pb.IsDone)
                        return $"{pb.Value} elements processed in {pb.TimeProcessing.TotalSeconds}s.";
                    else
                        return $"{pb.Percentage}%... Remaining: {pb.TimeRemaining?.TotalSeconds}s. - Current: {pb.ElementName}";
                });

                // Setting ProgressBar width
                pb.Layout.ProgressBarWidth = Console.BufferWidth;

                pb.Start();
                
                for (int i = 0; i < max; i++)
                {
                    string elementName = Guid.NewGuid().ToString();

                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
                }
            }
        }

        public static void Example11()
        {
            const int max = 500;

            //Create the ProgressBar
            using (var pb = new ProgressBar(autoStart: false) { Maximum = max })
            {
                // Hide Text
                pb.Text.Body.SetVisible(false);

                // Clear "Description Text"
                pb.Text.Description.Clear();

                // Hide "Description Indentation"
                pb.Text.Description.Indentation.SetVisible(false);

                // Setting "Description" when "Processing", with color
                pb.Text.Description.Processing.AddNew().SetValue(pb => $"{pb.ElementName}...")
                                                       .SetForegroundColor(ConsoleColor.DarkGray);
                

                // Hide "Margins"
                pb.Layout.Margins.SetVisible(false);

                // Set "Marquee" Color
                pb.Layout.Marquee.SetBackgroundColor(ConsoleColor.Black);

                // Setting Body Colors
                pb.Layout.Body.SetBackgroundColor(ConsoleColor.Black);
                pb.Layout.Body.Pending.SetForegroundColor(ConsoleColor.Red);
                pb.Layout.Body.Progress.SetForegroundColor(pb => pb.IsDone ? ConsoleColor.Cyan : ConsoleColor.Green);

                // Setting Body Text (internal text), from Layout
                string textDone = "----------------Done----------------";
                string text =     "-------------Processing-------------";
                pb.Layout.Body.Text.SetVisible(true).SetValue(pb => pb.IsDone ? textDone : text);

                // Setting ProgressBar width
                pb.Layout.ProgressBarWidth = text.Length;

                pb.Start();

                for (int i = 0; i < max; i++)
                {
                    string elementName = Guid.NewGuid().ToString();

                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(elementName); //Step in ProgressBar. Setting current ElementName
                }
            }
        }
    }
}
