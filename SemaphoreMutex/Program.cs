// Ignore Spelling: Mutex

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreMutex
{
    internal class Program
    {
        private static string _criticalResourcePath = "./resource.txt";
        private static object _lock = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Before every thing start");

            Console.Write("Before ");
            PrintCriticalResourceInfo(GetLastId());

            Console.WriteLine("Tasks are running ...");
            var tasks = new List<Task>();
            for (int i = 1; i <= 10; i++)
            {
                tasks.Add(Task.Run(Job));
            }

            Console.WriteLine("Waiting Waiting Waiting ");
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Tasks finished their works");

            Console.Write("After  ");
            PrintCriticalResourceInfo(GetLastId());

            Console.WriteLine("The End. Press a key ...");
            Console.ReadKey();
        }

        private static void Job()
        {
            lock (_lock)
            {
                Console.Write("Before ");
                int r = GetLastId();
                PrintCriticalResourceInfo(r);
                var rnd = new Random().Next(500, 2000);
                Thread.Sleep(rnd);

                r++;
                File.AppendAllText(_criticalResourcePath, $"{r}\r\n");

                Console.Write("After  ");
                PrintCriticalResourceInfo(r);
            }
        }

        private static int GetLastId()
        {
            var lastLine = File.ReadAllLines(_criticalResourcePath).LastOrDefault();
            int.TryParse(lastLine, out var id);
            return id;
        }

        private static void PrintCriticalResourceInfo(int resourceData)
        {
            Console.WriteLine($"# {Thread.CurrentThread.ManagedThreadId:D3} => {resourceData:D2}");
        }
    }
}
