using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Engine.Foundation;
using Engine.Models;

namespace Engine.Providers
{
    public class ImageInfoProvider : IMetaDataProvider<ImageMetaData>
    {
        private readonly IFileSystem fileSystem;
        private readonly IThumbnailCreator thumbnailCreator;
        private readonly IImageWrapper imageWrapper;

        public ImageInfoProvider(IThumbnailCreator thumbnailCreator, IFileSystem fileSystem, IImageWrapper imageWrapper)
        {
            this.thumbnailCreator = thumbnailCreator;
            this.fileSystem = fileSystem;
            this.imageWrapper = imageWrapper;
        }
        public async Task<ImageMetaData> ProvideAndCreateAsync(string imagePath, CancellationToken token)
        {
            int width, height;
            await using (var stream = this.fileSystem.File.Open(imagePath, FileMode.Open))
            using (var image = this.imageWrapper.FromStream(stream))
            {
                width = image.Width;
                height = image.Height;
            }

            var fileInfo = this.fileSystem.FileInfo.FromFileName(imagePath);

            var result = await this.thumbnailCreator.CreateAsync(imagePath, token);

            return new ImageMetaData(imagePath, result.OutputPath, width, height, fileInfo.Length);
        }
    }
}
