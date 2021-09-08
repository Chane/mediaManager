using System;
using System.Linq;
using Engine.Foundation;
using Engine.Models;

namespace Engine
{
    public class ThumbnailCacheLocationProvider : IThumbnailCacheLocationProvider
    {
        private readonly IWorkingDirectoryProvider workingDirectoryProvider;

        public ThumbnailCacheLocationProvider(IWorkingDirectoryProvider workingDirectoryProvider)
        {
            this.workingDirectoryProvider = workingDirectoryProvider;
        }

        public ThumbnailCacheLocation ProvideLocation(string sourceFilePath)
        {
            var executingDirectory = this.workingDirectoryProvider.CurrentExecutingDirectory();
            var cacheDirectory = string.Join("/", sourceFilePath.Split("/").SkipLast(1));

            Console.WriteLine($"Cache Directory   :: {cacheDirectory}");

            var directory = $"{executingDirectory}/_cache{cacheDirectory}";

            Console.WriteLine($"Directory         :: {directory}");

            var fileNameParts = sourceFilePath.Split("/").Last().Split(".").SkipLast(1);
            var fileName = string.Join(".", fileNameParts);

            Console.WriteLine($"File Name         :: {fileName}");

            return new ThumbnailCacheLocation(directory, fileName);
        }
    }
}
