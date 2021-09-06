using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Engine
{
    public class VideoInfoProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly IFFmpegWrapper fFmpegWrapper;
        private readonly IWorkingDirectoryProvider workingDirectoryProvider;

        public VideoInfoProvider(IFileSystem fileSystem, IFFmpegWrapper fFmpegWrapper, IWorkingDirectoryProvider workingDirectoryProvider)
        {
            this.fileSystem = fileSystem;
            this.fFmpegWrapper = fFmpegWrapper;
            this.workingDirectoryProvider = workingDirectoryProvider;
        }

        public async Task<VideoInfo> ProvideAndCreateAsync(string filePath, CancellationToken token)
        {
            var fileName = this.fileSystem.Path.GetFileName(filePath);
            var executingDirectory = this.workingDirectoryProvider.CurrentExecutingDirectory();

            string OutputFileNameBuilder(string number) => $"{executingDirectory}/{fileName}_thumb_" + number + ".png";

            var info = await this.fFmpegWrapper
                .GetMediaInfoAsync(filePath, token)
                .ConfigureAwait(false);
            var videoStream = info.VideoStreams
                .First()?
                .SetCodec(VideoCodec.png);

            var conversionResult = await this.fFmpegWrapper
                    .ExtractNthFrame(videoStream, OutputFileNameBuilder, 300, token)
                    .ConfigureAwait(false);

            return new VideoInfo(true, conversionResult.Duration);
        }

        public async Task<VideoInfo> CreateSnapshot(string filePath, int seconds, CancellationToken token)
        {
            var fileName = this.fileSystem.Path.GetFileName(filePath);
            var executingDirectory = workingDirectoryProvider.CurrentExecutingDirectory();

            var outputPath = $"{executingDirectory}/{fileName}_thumb.png";

            var conversionResult = await this.fFmpegWrapper
                .CreateSnapshot(filePath, seconds, outputPath, token)
                .ConfigureAwait(false);

            return new VideoInfo(true, conversionResult.Duration);
        }
    }

    public record VideoInfo(bool Created, TimeSpan Duration);
}
