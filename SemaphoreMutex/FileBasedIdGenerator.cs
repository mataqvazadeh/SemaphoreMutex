// Ignore Spelling: Mutex

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SemaphoreMutex
{
    internal class FileBasedIdGenerator : IIdGeneratorResource
    {
        private readonly string _filePath;

        public FileBasedIdGenerator(string filePath)
        {
            _filePath = filePath;
            CreateFileIfNotExists();
        }

        public int GetLatestId()
        {

            var lastLine = File.ReadAllLines(_filePath).LastOrDefault();
            int.TryParse(lastLine, out var id);
            
            return id;
        }

        private void CreateFileIfNotExists()
        {
            if (!File.Exists(_filePath))
            {
                File.AppendAllText(_filePath, string.Empty);
            }
        }

        public void SetLatestId(int id)
        {
            File.AppendAllText(_filePath, $"{id}\r\n");
        }
    }
}
