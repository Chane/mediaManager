using System;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Imageflow.Fluent;

namespace Engine
{
    public class ThumbnailCreator
    {
        private readonly IFileSystem fileSystem;
        private readonly IWorkingDirectoryProvider workingDirectoryProvider;

        public ThumbnailCreator(IFileSystem fileSystem, IWorkingDirectoryProvider workingDirectoryProvider)
        {
            this.fileSystem = fileSystem;
            this.workingDirectoryProvider = workingDirectoryProvider;
        }

        public async Task<ThumbnailResult> CreateAsync(byte[] imageBytes, CancellationToken token)
        {
            using var imageJob = new ImageJob();
            var encoder = new PngQuantEncoder();
            var result = await imageJob.Decode(imageBytes)
                .ConstrainWithin(120, 120)
                .EncodeToBytes(encoder)
                .Finish()
                .WithCancellationToken(token)
                .InProcessAsync()
                .ConfigureAwait(false);

            var bytes = result.First.TryGetBytes();
            var created = bytes.HasValue;

            if (created)
            {
                var executingDirectory = this.workingDirectoryProvider.CurrentExecutingDirectory();

                Console.WriteLine(executingDirectory);

                var fileName = "test"; // TODO: Pass in file name details
                var outputPath = $"{executingDirectory}/{fileName}_thumb.png";
                await this.fileSystem.File.WriteAllBytesAsync(outputPath,bytes.Value.Array, token);
            }

            return new ThumbnailResult(created);
        }
    }

    public record ThumbnailResult(bool Created);
}
