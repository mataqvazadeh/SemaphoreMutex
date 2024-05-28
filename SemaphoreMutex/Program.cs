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
            var mutex = new Mutex(true, @$"Global\MAT.SemaphoreMutex", out var createdNew);

            if (!createdNew)
            {
                mutex.WaitOne();
            }

            try
            {
                Console.Write("Before ");
                int r = GetLastId();
                PrintCriticalResourceInfo(r);
                var rnd = new Random().Next(1000, 5000);
                Thread.Sleep(rnd);

                r++;
                File.AppendAllText(_criticalResourcePath, $"{r}\r\n");

                Console.Write("After  ");
                PrintCriticalResourceInfo(r);

                rnd = new Random().Next(1000, 5000);
                Thread.Sleep(rnd);
            }
            finally
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
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
