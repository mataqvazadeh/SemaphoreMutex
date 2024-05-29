// Ignore Spelling: Mutex

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace SemaphoreMutex
{
    internal class IdGenerator
    {
        private const int SEMAPHORE_MAX_COUNT = 10;
        private const string MUTEX_NAME = "MAT.SemaphoreMutex.Mutex";
        
        private readonly static MixConcurrencyLock _lock;
        private readonly IIdGeneratorResource _resource;

        static IdGenerator()
        {
            _lock = new MixConcurrencyLock(SEMAPHORE_MAX_COUNT, MUTEX_NAME);
        }

        public IdGenerator(IIdGeneratorResource resource)
        {
            _resource = resource;
        }

        public void GenerateNextID()
        {
            if (!_lock.Wait())
            {
                return;
            }

            try
            {
                int id = _resource.GetLatestId();
                
                var rnd = new Random().Next(500, 1000);
                Thread.Sleep(rnd);

                id++;
                _resource.SetLatestId(id);

                rnd = new Random().Next(500, 1000);
                Thread.Sleep(rnd);

                PrintId(id);
            }
            finally
            {
                _lock.Release();
            }
        }

        private static void PrintId(int resourceData)
        {
            Console.WriteLine($"{IAm} => {resourceData:D3}");
        }

        private static string IAm => $"#P{Process.GetCurrentProcess().Id:D7}-#T{Thread.CurrentThread.ManagedThreadId:D3}";
    }
}
