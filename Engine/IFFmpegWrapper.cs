using System;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Engine
{
    public interface IFFmpegWrapper
    {
        Task<IMediaInfo> GetMediaInfoAsync(string filePath, CancellationToken token);

        Task<IConversionResult> ExtractNthFrame(
            IVideoStream videoStream,
            Func<string, string> outputFileNameBuilder,
            int frameNumber,
            CancellationToken token);

        Task<IConversionResult> CreateSnapshot(string sourcePath, int seconds, string outputPath, CancellationToken token);
    }
}
