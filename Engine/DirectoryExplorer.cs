using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Engine
{
    public class DirectoryExplorer
    {
        private readonly IFileSystem fileSystem;

        public DirectoryExplorer(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public IEnumerable<string> ListDirectories(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            }

            var directories = this.fileSystem.Directory.EnumerateDirectories(filePath, "*", SearchOption.AllDirectories).ToList();
            directories.Add(filePath);
            return directories;
        }
    }
}