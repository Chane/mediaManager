using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace Engine.Tests
{
    public class FileSystemBuilder
    {
        private MockFileSystem fileSystem;

        public IDictionary<string, MockFileData> SourceFilePaths { get; } = new Dictionary<string, MockFileData>();

        public IList<string> SourceDirectories { get; } = new List<string>();

        public FileSystemBuilder CreateDefaultFiles()
        {
            this.AddFiles("/myfile.txt", new MockFileData("Testing is meh."));
            this.AddFiles("/collection/video.mp4", new MockFileData("some js"));
            this.AddFiles("/collection/image.jpg", new MockFileData(new byte[] {0x12, 0x34, 0x56, 0xd2}));
            this.AddFiles("/collection/sub1/video.mp4", new MockFileData("some js"));
            this.AddFiles("/collection/sub1/image.jpg", new MockFileData(new byte[] {0x12, 0x34, 0x56, 0xd2}));
            this.AddFiles("/collection/sub2/video.mp4", new MockFileData("some js"));
            this.AddFiles("/collection/sub2/image.jpg", new MockFileData(new byte[] {0x12, 0x34, 0x56, 0xd2}));
            return this;
        }

        public FileSystemBuilder AddFiles(string filePath, MockFileData fileData)
        {
            this.SourceFilePaths.Add(filePath, fileData);

            var directory = string.Join('/', filePath.Split('/').SkipLast(1));
            if (!string.IsNullOrWhiteSpace(directory) && !this.SourceDirectories.Contains(directory))
            {
                this.SourceDirectories.Add(directory);
            }

            return this;
        }

        public IFileSystem Build()
        {
            this.fileSystem = new MockFileSystem();

            foreach (var fileData in SourceFilePaths)
            {
                this.fileSystem.AddFile(fileData.Key, fileData.Value);
            }

            return this.fileSystem;
        }
    }
}