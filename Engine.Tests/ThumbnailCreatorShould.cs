using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Engine.Foundation;
using Engine.Models;
using Engine.Tests.Helpers;
using Moq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class ThumbnailCreatorShould
    {
        [Test]
        public async Task CreateThumbnailByFilePath()
        {
            const string filePath = "/collection/new_image.jpg";
            var imageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABAQMAAAAl21bKAAAAA1BMVEX/TQBcNTh/AAAAAXRSTlPM0jRW/QAAAApJREFUeJxjYgAAAAYAAzY3fKgAAAAASUVORK5CYII=");

            var fileSystem = new FileSystemBuilder()
                .AddFiles(filePath, new MockFileData(imageBytes))
                .Build();

            var thumbnailCacheLocationMock = new Mock<IThumbnailCacheLocationProvider>();
            var cacheLocationResult = new ThumbnailCacheLocation("/collection", "new_image");
            thumbnailCacheLocationMock.Setup(m => m.ProvideLocation(filePath))
                .Returns(cacheLocationResult);

            var cancellationTokenSource = new CancellationTokenSource();
            var creator = new ThumbnailCreator(fileSystem, thumbnailCacheLocationMock.Object);

            var result = await creator.CreateAsync(
                filePath,
                cancellationTokenSource.Token);
            Assert.That(result.Created, Is.True);
        }

        [Test, Ignore("Integration Test")]
        public async Task Create_IntegrationTest()
        {
            var fileSystem = new FileSystem();
            var cancellationTokenSource = new CancellationTokenSource();
            var thumbnailCacheLocation = new ThumbnailCacheLocationProvider(new WorkingDirectoryProvider(fileSystem));
            var creator = new ThumbnailCreator(fileSystem, thumbnailCacheLocation);

            var filePath = string.Empty;

            var result = await creator.CreateAsync(
                filePath,
                cancellationTokenSource.Token);
            Assert.That(result.Created, Is.True);
        }
    }
}
