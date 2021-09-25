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
            var (directory, fileName) = destinationPath != null
                ? this.thumbnailCacheLocation.ProvideLocation(destinationPath)
                : this.thumbnailCacheLocation.ProvideLocation(filePath);
            var outputFile = $"{directory}/{fileName}_thumb.png";

            Debug.WriteLine($"Directory         :: {directory}");
            Debug.WriteLine($"Output File       :: {outputFile}");

            if (this.fileSystem.File.Exists(outputFile))
            {
                return new ThumbnailResult(false, outputFile);
            }

            using var imageJob = new ImageJob();
            var encoder = new PngQuantEncoder();
            var result = await imageJob.Decode(imageBytes)
                .Constrain(new Constraint(ConstraintMode.Within_Pad,120,120)
                {
                    CanvasColor = AnyColor.Transparent,
                    H = 120,
                    W = 120,
                    Mode = ConstraintMode.Within_Pad
                })
                .EncodeToBytes(encoder)
                .Finish()
                .WithCancellationToken(token)
                .InProcessAsync()
                .ConfigureAwait(false);

            var bytes = result.First.TryGetBytes();

            var created = bytes.HasValue;

            if (created)
            {
                this.fileSystem.Directory.CreateDirectory(this.fileSystem.Path.GetDirectoryName($"{directory}/"));
                await this.fileSystem.File.WriteAllBytesAsync(outputFile, bytes.Value.Array, token);
            }

            return new ThumbnailResult(created, outputFile);
        }
    }
}
