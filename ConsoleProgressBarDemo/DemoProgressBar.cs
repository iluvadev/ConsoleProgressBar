﻿using iluvadev.ConsoleProgressBar;
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

    }
}
