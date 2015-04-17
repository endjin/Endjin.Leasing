namespace Endjin.Leasing.Demo
{
    #region Using Directives

    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Endjin.Core.Composition;
    using Endjin.Core.Container;

    #endregion 

    public class Program
    {
        public static void Main(string[] args)
        {
            ApplicationServiceLocator.InitializeAsync(new Container(), new DesktopBootstrapper()).Wait();

            RunMutex();
            RunMutexWithOptions();

            Console.ReadKey();
        }

        private static void RunMutex()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("5 Actors will attempt to execute a long running process and will retry until they have all sucessfully executed.");
            Console.ResetColor();

            var tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                var actor = new Actor(string.Format("Actor {0}", i + 1));
                tasks[i] = actor.RunSimpleMutexAsync();
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Task.WaitAll(tasks);

            stopwatch.Stop();

            Console.WriteLine("Duration: " + stopwatch.ElapsedMilliseconds + " Miliseconds");
        }

        private static void RunMutexWithOptions()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("5 Actors will attempt to execute a long running process and will fail if they cannot obtain the lease first time.");
            Console.ResetColor();
            
            var tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                var actor = new Actor(string.Format("Actor {0}", i + 1));
                tasks[i] = actor.RunMutexWithOptionsAsync();
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Task.WaitAll(tasks);

            stopwatch.Stop();

            Console.WriteLine("Duration: " + stopwatch.ElapsedMilliseconds + " Miliseconds");
        }
    }
}
