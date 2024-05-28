// Ignore Spelling: Mutex

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreMutex
{
    internal class Program
    {
        private static string _criticalResourcePath = "./resource.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Tasks are running forever ...");
            while (true)
            {
                Thread.Sleep(1);
                Task.Run(Job);
            }
        }

        private static void Job()
        {
            Console.WriteLine($"{IAm} => Hello");
            var semaphore = new Semaphore(9, 10, @$"Global\MAT.SemaphoreMutex", out var createdNew);

            try
            {
                if (!createdNew)
                {
                    if (!semaphore.WaitOne(0))
                    {
                        Console.WriteLine($"{IAm} => I murdered by you.");
                        return;
                    }
                }

                int r = GetLastId();
                var rnd = new Random().Next(1000, 5000);
                Thread.Sleep(rnd);

                r++;
                File.AppendAllText(_criticalResourcePath, $"{r}\r\n");

                PrintCriticalResourceInfo(r);

                rnd = new Random().Next(1000, 5000);
                Thread.Sleep(rnd);
            }
            finally
            {
                semaphore.Release();
                semaphore.Dispose();
                Console.WriteLine($"{IAm} => Goodbye");
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
            Console.WriteLine($"{IAm} => {resourceData:D2}");
        }

        private static string IAm => $"#P{Process.GetCurrentProcess().Id:D8}-#T{Thread.CurrentThread.ManagedThreadId:D4}";
    }
}
