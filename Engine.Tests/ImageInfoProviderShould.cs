using System.Threading;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Providers;
using Moq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class ImageInfoProviderShould
    {
        [Test]
        public async Task CreateThumbnail()
        {
            const string filePath = "/image.jpg";
            const string thumbPath = "/image_thumb.jpg";
            var cancellationTokenSource = new CancellationTokenSource();

            var thumbnailResult = new ThumbnailResult(true, thumbPath);

            var thumbnailCreatorMock = new Mock<IThumbnailCreator>();
            thumbnailCreatorMock.Setup(m => m.CreateAsync(filePath, cancellationTokenSource.Token))
                .ReturnsAsync(thumbnailResult);

            var provider = new ImageInfoProvider(thumbnailCreatorMock.Object);
            var result = await provider.ProvideAndCreateAsync(filePath, cancellationTokenSource.Token);

            Assert.That(result.FilePath, Is.EqualTo(filePath));
            Assert.That(result.ThumbnailPath, Is.EqualTo(thumbPath));
            Assert.That(result.FileType, Is.EqualTo(FileType.Image));
        }
    }
}