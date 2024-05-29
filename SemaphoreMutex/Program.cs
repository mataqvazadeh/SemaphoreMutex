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

        static void Main(string[] args)
        {
            var resource = new FileBasedIdGenerator(CRITICAL_RESOURCE_PATH);
            var idGenerator = new IdGenerator(resource);
            
            Console.WriteLine("Tasks are running forever ...");
            while (true)
            {
                Thread.Sleep(1);
                Task.Run(idGenerator.GenerateNextID);
            }
        }
    }
}
