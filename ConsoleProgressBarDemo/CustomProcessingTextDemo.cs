using ConsoleProgressBar;
using System;
using System.Threading.Tasks;

namespace ConsoleProgressBarDemo
{
    public static class CustomProcessingTextDemo
    {
        public static void RunDemo()
        {
            string textDemo = "ProgressBar with Custom Processing Text";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.Clear();
                Console.WriteLine(new string('-', textDemo.Length + 2));
                Console.WriteLine($" {textDemo} ");
                Console.WriteLine(new string('-', textDemo.Length + 2));
            }
            Task[] tasks = new Task[] {
                new Task(() => HideAlways(5)),
                new Task(() => ContextualText(10)),
                new Task(() => FixedText(15)),
            };

            foreach (Task task in tasks)
                task.Start();

            Task.WaitAll(tasks);
        }

        public static void HideAlways(int initialLine = 0)
        {
            const int max = 250;

            string textDemo = $"ProgressBar without Text while processing";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.SetCursorPosition(0, initialLine);
                Console.WriteLine($"- {textDemo}:");
            }

            //Create the ProgressBar (initialLine is optional, last Console line is assumed)
            using (var pb = new ProgressBar(initialLine + 1) { Maximum = max })
            {
                pb.Text.Processing.SetVisible(false); //Set always Visible = false
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(25).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }



        public static void FixedText(int initialLine = 0)
        {
            const int max = 250;

            string textDemo = $"Default ProgressBar with fixed Text while Processing";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.SetCursorPosition(0, initialLine);
                Console.WriteLine($"- {textDemo}:");
            }

            //Create the ProgressBar (initialLine is optional, last Console line is assumed)
            using (var pb = new ProgressBar(initialLine + 1) { Maximum = null })
            {
                pb.Text.Processing.SetValue("Processing, please wait..."); //Set fixed Processing text
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(25).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void ContextualText(int initialLine = 0)
        {
            const int max = 250;

            string textDemo = $"Default ProgressBar with contextual Text while Processing";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.SetCursorPosition(0, initialLine);
                Console.WriteLine($"- {textDemo}:");
            }

            //Create the ProgressBar (initialLine is optional, last Console line is assumed)
            using (var pb = new ProgressBar(initialLine + 1) { Maximum = max })
            {
                pb.Text.Processing.SetValue(b => $"Processing petition {b.Value}... (runing time: {b.TimeProcessing})"); //Set contextual Processing text
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(25).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

    }
}
