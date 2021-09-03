using System;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Engine
{
    public class VideoInfoProvider
    {
        private readonly IFileSystem fileSystem;

        public VideoInfoProvider(IFileSystem fileSystem) => this.fileSystem = fileSystem;

        public async Task<VideoInfo> ProvideAndCreateAsync(string filePath, CancellationToken token)
        {
            var fileName = this.fileSystem.Path.GetFileName(filePath);
            var executingDirectory = this.fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string OutputFileNameBuilder(string number) => $"{executingDirectory}/{fileName}_thumb_" + number + ".png";

            IMediaInfo info = await FFmpeg.GetMediaInfo(filePath, token).ConfigureAwait(false);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.png);

            IConversionResult conversionResult = await FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .ExtractNthFrame(300, OutputFileNameBuilder)
                .Start(token)
                .ConfigureAwait(false);

            return new VideoInfo(true, conversionResult.Duration);
        }
    }

    public record VideoInfo(bool Created, TimeSpan Duration);
}