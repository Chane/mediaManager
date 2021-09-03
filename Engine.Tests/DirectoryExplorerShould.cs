using System.Linq;
using NUnit.Framework;

namespace Engine.Tests
{
    public class DirectoryExplorerShould
    {
        // Create Thumbnail for Images
        // Create Thumbnail for Video
        // List Files in Directories
        // Create MetaData for Files in Directories
        // List MetaData for a directory

        [Test]
        public void ListDirectories()
        {
            var builder = new FileSystemBuilder();
            var fileSystem = builder
                .CreateDefaultFiles()
                .Build();

            var explorer = new DirectoryExplorer(fileSystem);

            var searchDirectory = "collection";

            var directories = explorer.ListDirectories($"/{searchDirectory}").ToList();

            foreach (var directory in builder.SourceDirectories)
            {
                Assert.That(directories, Contains.Item(directory));
            }

            Assert.That(directories.Count, Is.EqualTo(builder.SourceDirectories.Count));
        }

        [Test]
        public void ListAllFilesInDirectory()
        {
            var builder = new FileSystemBuilder();
            var fileSystem = builder
                .CreateDefaultFiles()
                .Build();

            var explorer = new DirectoryExplorer(fileSystem);

            var searchDirectory = "/collection";

            var files = explorer.ListFiles(searchDirectory).ToList();

            var expectedFiles = builder.SourceFilePaths
                .Where(key => !key.Key.Contains("/sub") && key.Key.StartsWith(searchDirectory)).ToList();

            foreach (var file in expectedFiles)
            {
                Assert.That(files, Contains.Item(file.Key));
            }

            Assert.That(files.Count, Is.EqualTo(expectedFiles.Count));
        }
    }
}