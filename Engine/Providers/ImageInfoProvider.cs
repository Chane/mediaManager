using System.Threading;
using System.Threading.Tasks;
using Engine.Models;

namespace Engine.Providers
{
    public class ImageInfoProvider : IMetaDataProvider<ImageMetaData>
    {
        private readonly IThumbnailCreator thumbnailCreator;

        public ImageInfoProvider(IThumbnailCreator thumbnailCreator)
        {
            this.thumbnailCreator = thumbnailCreator;
        }

        public async Task<ImageMetaData> ProvideAndCreateAsync(string imagePath, CancellationToken token)
        {
            // TODO: read Info

            var result = await this.thumbnailCreator.CreateAsync(imagePath, token);

            return new ImageMetaData(imagePath, result.OutputPath, 0, 0, 0, string.Empty);
        }
    }
}
