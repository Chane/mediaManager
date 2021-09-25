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
        private readonly byte[] imageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABAQMAAAAl21bKAAAAA1BMVEX/TQBcNTh/AAAAAXRSTlPM0jRW/QAAAApJREFUeJxjYgAAAAYAAzY3fKgAAAAASUVORK5CYII=");

        [Test]
        public async Task CreateThumbnailByFilePath()
        {
            const string basePart = "/collection/new_image";
            const string thumbnail = basePart + "_thumb.png";
            const string filePath = basePart + ".jpg";

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
            Assert.That(result.OutputPath, Is.EqualTo(thumbnail));
        }

        [Test]
        public async Task CreateThumbnailByFilePathToSpecificDestination()
        {
            const string fileName = "new_image";
            const string destination = "/_cache/here/please";
            const string filePath = "/collection/" + fileName + ".jpg";

            var fileSystem = new FileSystemBuilder()
                .AddFiles(filePath, new MockFileData(imageBytes))
                .Build();

            var thumbnailCacheLocationMock = new Mock<IThumbnailCacheLocationProvider>();
            var cacheLocationResult = new ThumbnailCacheLocation(destination, fileName);
            thumbnailCacheLocationMock.Setup(m => m.ProvideLocation(destination))
                .Returns(cacheLocationResult);

            var cancellationTokenSource = new CancellationTokenSource();
            var creator = new ThumbnailCreator(fileSystem, thumbnailCacheLocationMock.Object);

            var result = await creator.CreateAsync(
                filePath,
                destination,
                cancellationTokenSource.Token);
            Assert.That(result.Created, Is.True);
            Assert.That(result.OutputPath, Is.EqualTo($"{destination}/{fileName}_thumb.png"));
        }

        [Test]
        public async Task NotCreateIfTheFileAlreadyExists()
        {
            const string fileName = "new_image";
            const string destination = "/_cache/here/please";
            const string filePath = "/collection/" + fileName + ".jpg";
            const string thumbnailPath = destination + "/" + fileName + "_thumb.png";

            var fileSystem = new FileSystemBuilder()
                .AddFiles(thumbnailPath, new MockFileData(imageBytes))
                .AddFiles(filePath, new MockFileData(imageBytes))
                .Build();

            var thumbnailCacheLocationMock = new Mock<IThumbnailCacheLocationProvider>();
            var cacheLocationResult = new ThumbnailCacheLocation(destination, fileName);
            thumbnailCacheLocationMock.Setup(m => m.ProvideLocation(destination))
                .Returns(new ThumbnailCacheLocation(destination, fileName));
            thumbnailCacheLocationMock.Setup(m => m.ProvideLocation(filePath))
                .Returns(new ThumbnailCacheLocation(filePath, fileName));

            var cancellationTokenSource = new CancellationTokenSource();
            var creator = new ThumbnailCreator(fileSystem, thumbnailCacheLocationMock.Object);

            var result = await creator.CreateAsync(
                filePath,
                destination,
                cancellationTokenSource.Token);

            Assert.That(result.Created, Is.False);
            Assert.That(result.OutputPath, Is.EqualTo(thumbnailPath));
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
