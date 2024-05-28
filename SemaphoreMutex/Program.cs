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
            var semaphore = new Semaphore(9, 10, @$"Global\MAT.SemaphoreMutex.Semaphore", out var createdNewSemaphore);

            try
            {
                if (!createdNewSemaphore)
                {
                    if (!semaphore.WaitOne(0))
                    {
                        return;
                    }
                }

                var mutex = new Mutex(true, @$"Global\MAT.SemaphoreMutex.Mutex", out var createdNewMutex);

                if (!createdNewMutex)
                {
                    mutex.WaitOne();
                }

                try
                {
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
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
            finally
            {
                semaphore.Release();
                semaphore.Dispose();
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
