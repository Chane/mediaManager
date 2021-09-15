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
            string FileName(string filePath)
            {
                var fileNameParts = filePath.Split("/").Last().Split(".").SkipLast(1);
                var fileName = string.Join(".", fileNameParts);
                return fileName;
            }

            ThumbnailCacheLocation thumbnailCacheLocation;
            if (sourceFilePath.Contains("_cache"))
            {
                var cacheDirectory = string.Join("/", sourceFilePath.Split("/").SkipLast(1));
                var fileName = FileName(sourceFilePath);
                thumbnailCacheLocation = new ThumbnailCacheLocation(cacheDirectory, fileName);
            }
            else
            {
                var executingDirectory = this.workingDirectoryProvider.CurrentExecutingDirectory();
                var cacheDirectory = string.Join("/", sourceFilePath.Split("/").SkipLast(1));

                Console.WriteLine($"Cache Directory   :: {cacheDirectory}");

                var directory = $"{executingDirectory}/_cache{cacheDirectory}";

                Console.WriteLine($"Directory         :: {directory}");

                var fileName = FileName(sourceFilePath);

                Console.WriteLine($"File Name         :: {fileName}");

                thumbnailCacheLocation = new ThumbnailCacheLocation(directory, fileName);
            }

            return thumbnailCacheLocation;
        }
    }
}
