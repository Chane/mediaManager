using System.Diagnostics;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Engine.Foundation;
using Engine.Models;

namespace Engine.Providers
{
    public class VideoInfoProvider : IVideoInfoProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly IFFmpegWrapper fFmpegWrapper;
        private readonly IThumbnailCacheLocationProvider thumbnailCacheLocationProvider;
        private readonly IThumbnailCreator thumbnailCreator;

        public VideoInfoProvider(IFileSystem fileSystem, IFFmpegWrapper fFmpegWrapper, IThumbnailCacheLocationProvider thumbnailCacheLocationProvider, IThumbnailCreator thumbnailCreator)
        {
            this.fileSystem = fileSystem;
            this.fFmpegWrapper = fFmpegWrapper;
            this.thumbnailCacheLocationProvider = thumbnailCacheLocationProvider;
            this.thumbnailCreator = thumbnailCreator;
        }

        public async Task<VideoMetaData> ProvideAndCreateAsync(string filePath, CancellationToken token)
        {
            var info = await this.fFmpegWrapper
                .GetMediaInfoAsync(filePath, token)
                .ConfigureAwait(false);

            var result = await CreateSnapshot(filePath, 30, token);

            await this.thumbnailCreator.CreateAsync(result.SnapshotPath, result.SnapshotPath.Replace("_snap", string.Empty), token);

            this.fileSystem.File.Delete(result.SnapshotPath);

            return new VideoMetaData(filePath, result.SnapshotPath, 0, 0, info.Size, info.Duration);
        }

        public async Task<SnapshotResult> CreateSnapshot(string filePath, int seconds, CancellationToken token)
        {
            var cacheLocation = this.thumbnailCacheLocationProvider.ProvideLocation(filePath);

            var outputPath = $"{cacheLocation.Directory}/{cacheLocation.FileName}_snap.png";

            Debug.WriteLine($"Output Path :: {outputPath}");

            var conversionResult = await this.fFmpegWrapper
                .CreateSnapshot(filePath, seconds, outputPath, token)
                .ConfigureAwait(false);

            return new SnapshotResult(true, outputPath, conversionResult.Duration);
        }
    }
}
