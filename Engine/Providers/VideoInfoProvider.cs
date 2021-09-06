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
        private readonly IWorkingDirectoryProvider workingDirectoryProvider;
        private readonly IThumbnailCreator thumbnailCreator;

        public VideoInfoProvider(IFileSystem fileSystem, IFFmpegWrapper fFmpegWrapper, IWorkingDirectoryProvider workingDirectoryProvider, IThumbnailCreator thumbnailCreator)
        {
            this.fileSystem = fileSystem;
            this.fFmpegWrapper = fFmpegWrapper;
            this.workingDirectoryProvider = workingDirectoryProvider;
            this.thumbnailCreator = thumbnailCreator;
        }

        public async Task<VideoMetaData> ProvideAndCreateAsync(string filePath, CancellationToken token)
        {
            var info = await this.fFmpegWrapper
                .GetMediaInfoAsync(filePath, token)
                .ConfigureAwait(false);

            var result = await CreateSnapshot(filePath, 30, token);

            await this.thumbnailCreator.CreateAsync(result.SnapshotPath, token);

            return new VideoMetaData(filePath, result.SnapshotPath, 0, 0, info.Size, info.Duration);
        }

        public async Task<SnapshotResult> CreateSnapshot(string filePath, int seconds, CancellationToken token)
        {
            var fileName = this.fileSystem.Path.GetFileName(filePath);
            var executingDirectory = workingDirectoryProvider.CurrentExecutingDirectory();

            var outputPath = $"{executingDirectory}/{fileName}_thumb.png";

            var conversionResult = await this.fFmpegWrapper
                .CreateSnapshot(filePath, seconds, outputPath, token)
                .ConfigureAwait(false);

            return new SnapshotResult(true, outputPath, conversionResult.Duration);
        }
    }
}
