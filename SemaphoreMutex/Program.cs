// Ignore Spelling: Mutex

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreMutex
{
    internal class Program
    {
        private static int _criticalResource = 0;
        private static object _lock = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Before every thing start");
            Console.Write("Before ");
            PrintCriticalResourceInfo();

            Console.WriteLine("Tasks are running ...");
            var tasks = new List<Task>();
            for(int i = 1; i <= 10; i++)
            {
                tasks.Add(Task.Run(Job));
            }

            Console.WriteLine("Waiting Waiting Waiting ");
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Tasks finished their works");

            Console.Write("After  ");
            PrintCriticalResourceInfo();

            Console.WriteLine("The End. Press a key ...");
            Console.ReadKey();
        }

        private static void Job()
        {
            lock (_lock)
            {
                Console.Write("Before ");
                PrintCriticalResourceInfo();
                var r = _criticalResource;
                var rnd = new Random().Next(500, 2000);
                Thread.Sleep(rnd);

                _criticalResource = r + 1;

                Console.Write("After  ");
                PrintCriticalResourceInfo(); 
            }
        }

        private static void PrintCriticalResourceInfo()
        {
            Console.WriteLine($"# {Thread.CurrentThread.ManagedThreadId:D3} => {_criticalResource:D2}");
        }
    }
}
