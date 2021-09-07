using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Engine.Foundation;
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
            var filePath = "/collection/new_image.jpg";
            var imageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABAQMAAAAl21bKAAAAA1BMVEX/TQBcNTh/AAAAAXRSTlPM0jRW/QAAAApJREFUeJxjYgAAAAYAAzY3fKgAAAAASUVORK5CYII=");

            var fileSystem = new FileSystemBuilder()
                .AddFiles(filePath, new MockFileData(imageBytes))
                .Build();

            var workingDirectoryProviderMock = new Mock<IWorkingDirectoryProvider>();
            workingDirectoryProviderMock.Setup(m => m.CurrentExecutingDirectory()).Returns("/collection");

            var cancellationTokenSource = new CancellationTokenSource();
            var creator = new ThumbnailCreator(fileSystem, workingDirectoryProviderMock.Object);

            var result = await creator.CreateAsync(
                filePath,
                cancellationTokenSource.Token);
            Assert.That(result.Created, Is.True);
        }

        [Test]
        public async Task CreateThumbnail()
        {
            var fileSystem = new FileSystemBuilder()
                .CreateDefaultFiles()
                .Build();

            var workingDirectoryProviderMock = new Mock<IWorkingDirectoryProvider>();
            workingDirectoryProviderMock.Setup(m => m.CurrentExecutingDirectory()).Returns("/collection");

            var cancellationTokenSource = new CancellationTokenSource();
            var creator = new ThumbnailCreator(fileSystem, workingDirectoryProviderMock.Object);

            var imageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABAQMAAAAl21bKAAAAA1BMVEX/TQBcNTh/AAAAAXRSTlPM0jRW/QAAAApJREFUeJxjYgAAAAYAAzY3fKgAAAAASUVORK5CYII=");

            var result = await creator.CreateAsync(
                imageBytes,
                cancellationTokenSource.Token);
            Assert.That(result.Created, Is.True);
        }

        [Test, Ignore("Integration Test")]
        public async Task Create_IntegrationTest()
        {
            var fileSystem = new FileSystem();
            var cancellationTokenSource = new CancellationTokenSource();
            var creator = new ThumbnailCreator(fileSystem, new WorkingDirectoryProvider(fileSystem));

            var filePath = string.Empty;
            var imageBytes = await fileSystem.File.ReadAllBytesAsync(filePath, cancellationTokenSource.Token);

            var result = await creator.CreateAsync(
                imageBytes,
                cancellationTokenSource.Token);
            Assert.That(result.Created, Is.True);
        }
    }
}
