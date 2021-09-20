using ConsoleProgressBar;
using System;
using System.Threading.Tasks;

namespace ConsoleProgressBarDemo
{
    public static class DefaultProgressBarDemo
    {
        public static void RunDemo()
        {
            string textDemo = "ProgressBar with Default configuration";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.Clear();
                Console.WriteLine(new string('-', textDemo.Length + 2));
                Console.WriteLine($" {textDemo} ");
                Console.WriteLine(new string('-', textDemo.Length + 2));
            }
            Task[] tasks = new Task[] {
                new Task(() => WithMaximum(5)),
                new Task(() => WithMaximumAndStep(10)),
                new Task(() => UnknownMaximum(15)),
            };

            foreach (Task task in tasks)
                task.Start();

            Task.WaitAll(tasks);
        }

        public static void WithMaximum(int initialLine = 0)
        {
            const int max = 250;

            string textDemo = $"Default ProgressBar with Maximum ({max})";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.SetCursorPosition(0, initialLine);
                Console.WriteLine($"- {textDemo}:");
            }

            //Create the ProgressBar (initialLine is optional, last Console line is assumed)
            using (var pb = new ProgressBar(initialLine + 1) { Maximum = max })
            {
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(25).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void UnknownMaximum(int initialLine = 0)
        {
            const int max = 250;

            string textDemo = $"Default ProgressBar with unknown Maximum (null)";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.SetCursorPosition(0, initialLine);
                Console.WriteLine($"- {textDemo}:");
            }

            //Create the ProgressBar (initialLine is optional, last Console line is assumed)
            using (var pb = new ProgressBar(initialLine + 1) { Maximum = null })
            {
                for (int i = 0; i < max; i++)
                {
                    Task.Delay(25).Wait(); //Do something
                    pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void WithMaximumAndStep(int initialLine = 0)
        {
            const int max = 250;
            const int step = 25;

            string textDemo = $"Default ProgressBar with Maximum ({max}) and Step ({step})";
            lock (ProgressBar.ConsoleWriterLock)
            {
                Console.SetCursorPosition(0, initialLine);
                Console.WriteLine($"- {textDemo}:");
            }

            //Create the ProgressBar (initialLine is optional, last Console line is assumed)
            using (var pb = new ProgressBar(initialLine + 1) { Maximum = max, Step = step })
            {
                for (int i = 0; i <= max; i++)
                {
                    Task.Delay(25).Wait(); //Do something
                    if (i > 0 && i % step == 0)
                        pb.PerformStep(); //Step in ProgressBar (Default is 1)
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

    }
}
