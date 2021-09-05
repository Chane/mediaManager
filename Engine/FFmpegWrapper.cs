using System;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Engine
{
    public class FFmpegWrapper : IFFmpegWrapper
    {
        public async Task<IMediaInfo> GetMediaInfoAsync(string filePath, CancellationToken token)
        {
            return await FFmpeg.GetMediaInfo(filePath, token).ConfigureAwait(false);
        }

        public async Task<IConversionResult> ExtractNthFrame(
            IVideoStream videoStream,
            Func<string, string> outputFileNameBuilder,
            int frameNumber,
            CancellationToken token)
        {
            return await FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .ExtractNthFrame(frameNumber, outputFileNameBuilder)
                .Start(token)
                .ConfigureAwait(false);
        }

        public async Task<IConversionResult> CreateSnapshot(string sourcePath, int seconds, string outputPath, CancellationToken token)
        {
            var conversion = await FFmpeg.Conversions
                .FromSnippet
                .Snapshot(sourcePath, outputPath, TimeSpan.FromSeconds(seconds));
            return await conversion.Start(token);
        }
    }
}
