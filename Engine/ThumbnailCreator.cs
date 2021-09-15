using System.Diagnostics;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Engine.Models;
using Imageflow.Fluent;

namespace Engine
{
    public class ThumbnailCreator : IThumbnailCreator
    {
        private readonly IFileSystem fileSystem;
        private readonly IThumbnailCacheLocationProvider thumbnailCacheLocation;

        public ThumbnailCreator(IFileSystem fileSystem, IThumbnailCacheLocationProvider thumbnailCacheLocation)
        {
            this.fileSystem = fileSystem;
            this.thumbnailCacheLocation = thumbnailCacheLocation;
        }

        public async Task<ThumbnailResult> CreateAsync(string filePath, CancellationToken token)
        {
            var imageBytes = await fileSystem.File.ReadAllBytesAsync(filePath, token);
            return await this.CreateAsync(filePath, null, imageBytes, token);
        }

        public async Task<ThumbnailResult> CreateAsync(string filePath, string destinationPath, CancellationToken token)
        {
            var imageBytes = await fileSystem.File.ReadAllBytesAsync(filePath, token);
            return await this.CreateAsync(filePath, destinationPath, imageBytes, token);
        }

        private async Task<ThumbnailResult> CreateAsync(string filePath, string destinationPath, byte[] imageBytes, CancellationToken token)
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
            var outputFile = string.Empty;

            if (created)
            {
                var (directory, fileName) = destinationPath != null
                    ? this.thumbnailCacheLocation.ProvideLocation(destinationPath)
                    : this.thumbnailCacheLocation.ProvideLocation(filePath);

                Debug.WriteLine($"Directory         :: {directory}");

                this.fileSystem.Directory.CreateDirectory(this.fileSystem.Path.GetDirectoryName($"{directory}/"));

                outputFile = $"{directory}/{fileName}_thumb.png";
                Debug.WriteLine($"Output File       :: {outputFile}");

                await this.fileSystem.File.WriteAllBytesAsync(outputFile, bytes.Value.Array, token);
            }

            return new ThumbnailResult(created, outputFile);
        }
    }
}
