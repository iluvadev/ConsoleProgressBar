using iluvadev.ConsoleProgressBar;
using System;
using System.Collections.Generic;
using System.Text;
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
            using (var pb = new ProgressBar(initialPosition: 3, autoStart: false) { Maximum = max, Step = 2})
            {
                pb.Start(); // autoStart=false -> we need start manually
                for (int i = 0; i < max; i+=pb.Step)
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
                pb.Text.Body.Processing.SetVisible(false); //Set always Visible = false
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(10).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
        }

    }
}
