// Ignore Spelling: Mutex

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreMutex
{
    internal class Program
    {
        private const string CRITICAL_RESOURCE_PATH = "./resource.txt";
        private const int SEMAPHORE_MAX_COUNT = 10;
        private static SemaphoreSlim _semaphore;

        static void Main(string[] args)
        {
            _semaphore = new SemaphoreSlim(SEMAPHORE_MAX_COUNT, SEMAPHORE_MAX_COUNT);
            Console.WriteLine("Tasks are running forever ...");
            while (true)
            {
                Thread.Sleep(1);
                Task.Run(Job);
            }
        }

        private static void Job()
        {
            if (_semaphore.CurrentCount == 0)
            {
                return;
            }

            _semaphore.Wait();

            try
            {
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
                    File.AppendAllText(CRITICAL_RESOURCE_PATH, $"{r}\r\n");

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
                _semaphore.Release();
            }
        }

        private static int GetLastId()
        {
            var lastLine = File.ReadAllLines(CRITICAL_RESOURCE_PATH).LastOrDefault();
            int.TryParse(lastLine, out var id);
            return id;
        }

        private static void PrintCriticalResourceInfo(int resourceData)
        {
            Console.WriteLine($"{IAm} => {resourceData:D3}");
        }

        private static string IAm => $"#P{Process.GetCurrentProcess().Id:D7}-#T{Thread.CurrentThread.ManagedThreadId:D3}";
    }
}
