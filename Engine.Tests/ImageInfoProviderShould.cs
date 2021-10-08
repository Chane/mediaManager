using System;
using System.Drawing;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Engine.Foundation;
using Engine.Models;
using Engine.Providers;
using Engine.Tests.Helpers;
using Moq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class ImageInfoProviderShould
    {
        // This is a 95 byte 1x1 image
        private readonly byte[] imageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABAQMAAAAl21bKAAAAA1BMVEX/TQBcNTh/AAAAAXRSTlPM0jRW/QAAAApJREFUeJxjYgAAAAYAAzY3fKgAAAAASUVORK5CYII=");
        private const int Width = 1;
        private const int Height = 1;
        private const int Size = 95;

        [Test]
        public async Task CreateThumbnail()
        {
            const string filePath = "/image.jpg";
            const string thumbPath = "/image_thumb.jpg";
            var cancellationTokenSource = new CancellationTokenSource();

            var fileSystem = new FileSystemBuilder()
                .AddFiles(filePath, new MockFileData(imageBytes))
                .Build();

            var thumbnailResult = new ThumbnailResult(true, thumbPath);

            var thumbnailCreatorMock = new Mock<IThumbnailCreator>();
            thumbnailCreatorMock.Setup(m => m.CreateAsync(filePath, cancellationTokenSource.Token))
                .ReturnsAsync(thumbnailResult);

            await using var imageStream = new MemoryStream(imageBytes);
            var image = Image.FromStream(imageStream);

            var imageWrapperMock = new Mock<IImageWrapper>();
            imageWrapperMock.Setup(m => m.FromStream(It.IsAny<Stream>())).Returns(image);

            var provider = new ImageInfoProvider(thumbnailCreatorMock.Object, fileSystem, imageWrapperMock.Object);
            var result = await provider.ProvideAndCreateAsync(filePath, cancellationTokenSource.Token);

            Assert.Multiple(() =>
            {
                Assert.That(result.FilePath, Is.EqualTo(filePath));
                Assert.That(result.ThumbnailPath, Is.EqualTo(thumbPath));
                Assert.That(result.FileType, Is.EqualTo(FileType.Image));
                Assert.That(result.Height, Is.EqualTo(Width));
                Assert.That(result.Width, Is.EqualTo(Height));
                Assert.That(result.FileSize, Is.EqualTo(Size));
            });
        }
    }
}