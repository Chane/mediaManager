using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;

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

        public IEnumerable<string> ListFiles(string filePath, string searchPattern = "*")
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(searchPattern));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            }

            var files = this.fileSystem.Directory.EnumerateFiles(filePath, searchPattern,
                SearchOption.TopDirectoryOnly);

            return files;
        }
    }
}