// Ignore Spelling: Mutex

using System;
using System.Threading;

namespace SemaphoreMutex
{
    public class MixConcurrencyLock : IDisposable
    {
        private readonly SemaphoreSlim _semaphre;
        private readonly Mutex _mutex;
        private static readonly object _lock = new object();

        public MixConcurrencyLock(int maxCount, string mutexName = null)
        {
            _semaphre = new SemaphoreSlim(maxCount, maxCount);
            var name = string.IsNullOrEmpty(mutexName?.Trim())
                       ? null
                       : $@"Global\{GetType().FullName}.{mutexName}";
            _mutex = new Mutex(false, name);
        }

        public bool Wait()
        {
            lock (_lock)
            {
                if (_semaphre.CurrentCount == 0)
                {
                    return false;
                }
            }

            _semaphre.Wait();
            _mutex.WaitOne();

            return true;
        }

        public void Release()
        {
            _mutex.ReleaseMutex();
            _semaphre.Release();
        }

        public void Dispose()
        {
            _semaphre.Dispose();
            _mutex.Dispose();
        }
    }
}
