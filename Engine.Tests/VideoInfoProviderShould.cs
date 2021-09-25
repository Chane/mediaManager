using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Engine.Foundation;
using Engine.Models;
using Engine.Providers;
using Engine.Tests.Helpers;
using Moq;
using NUnit.Framework;
using Xabe.FFmpeg;

namespace Engine.Tests
{
    [TestFixture]
    public class VideoInfoProviderShould
    {
        private readonly byte[] imageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABAQMAAAAl21bKAAAAA1BMVEX/TQBcNTh/AAAAAXRSTlPM0jRW/QAAAApJREFUeJxjYgAAAAYAAzY3fKgAAAAASUVORK5CYII=");

        [Test]
        public async Task ProvideAndCreate()
        {
            var tokenSource = new CancellationTokenSource();
            var fileSystem = new MockFileSystem();
            var filePath = string.Empty;

            var mediaInfoMock = new Mock<IMediaInfo>();
            mediaInfoMock.Setup(m => m.VideoStreams)
                .Returns(new List<IVideoStream> { Mock.Of<IVideoStream>() });

            var conversionResultMock = new Mock<IConversionResult>();
            conversionResultMock.Setup(m => m.Duration)
                .Returns(default(TimeSpan));

            var ffmpegWrapperMock = new Mock<IFFmpegWrapper>();
            ffmpegWrapperMock.Setup(m => m.CreateSnapshot(filePath,
                    30,
                    It.IsAny<string>(),
                    tokenSource.Token))
                .ReturnsAsync(conversionResultMock.Object);
            ffmpegWrapperMock.Setup(m => m.GetMediaInfoAsync(It.IsAny<string>(), tokenSource.Token))
                .ReturnsAsync(mediaInfoMock.Object);

            var thumbnailCacheLocationProviderMock = new Mock<IThumbnailCacheLocationProvider>();
            thumbnailCacheLocationProviderMock.Setup(m => m.ProvideLocation(It.IsAny<string>()))
                .Returns(new ThumbnailCacheLocation(string.Empty, string.Empty));

            var thumbnailCreatorMock = new Mock<IThumbnailCreator>();
            thumbnailCreatorMock.Setup(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), tokenSource.Token))
                .ReturnsAsync(new ThumbnailResult(true, "outputPath"));

            var provider = new VideoInfoProvider(
                fileSystem,
                ffmpegWrapperMock.Object,
                thumbnailCacheLocationProviderMock.Object,
                thumbnailCreatorMock.Object);
            var videoDetail = await provider.ProvideAndCreateAsync(filePath, tokenSource.Token).ConfigureAwait(false);

            Assert.That(videoDetail, Is.Not.Null);
        }

        [Test]
        public async Task NotCreateThumbnailIfItAlreadyExists()
        {
            const string fileName = "new_image";
            const string destination = "/_cache/here/please";
            const string filePath = "/collection/" + fileName + ".jpg";
            const string thumbnailPath = destination + "/" + fileName + "_thumb.png";

            var fileSystem = new FileSystemBuilder()
                .AddFiles(thumbnailPath, new MockFileData(imageBytes))
                .Build();

            var tokenSource = new CancellationTokenSource();

            var thumbnailCacheLocationProviderMock = new Mock<IThumbnailCacheLocationProvider>();
            thumbnailCacheLocationProviderMock.Setup(m => m.ProvideLocation(It.IsAny<string>()))
                .Returns(new ThumbnailCacheLocation(destination, fileName));

            var thumbnailCreatorMock = new Mock<IThumbnailCreator>();

            var conversionResultMock = new Mock<IConversionResult>();
            conversionResultMock.Setup(m => m.Duration)
                .Returns(default(TimeSpan));

            var mediaInfoMock = new Mock<IMediaInfo>();
            mediaInfoMock.Setup(m => m.VideoStreams)
                .Returns(new List<IVideoStream> { Mock.Of<IVideoStream>() });

            var ffmpegWrapperMock = new Mock<IFFmpegWrapper>();
            ffmpegWrapperMock.Setup(m => m.GetMediaInfoAsync(It.IsAny<string>(), tokenSource.Token))
                .ReturnsAsync(mediaInfoMock.Object);

            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapperMock.Object, thumbnailCacheLocationProviderMock.Object, thumbnailCreatorMock.Object);
            var videoMetaData = await provider.ProvideAndCreateAsync(filePath, tokenSource.Token)
                .ConfigureAwait(false);

            thumbnailCreatorMock.Verify(v => v.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), tokenSource.Token), Times.Never);
            Assert.That(videoMetaData.ThumbnailPath, Is.EqualTo(thumbnailPath));
        }
        [Test]
        public async Task CreateSnapshot()
        {
            const string filePath = "/a/path";
            var tokenSource = new CancellationTokenSource();
            var fileSystem = new MockFileSystem();

            var conversionResultMock = new Mock<IConversionResult>();
            conversionResultMock.Setup(m => m.Duration).Returns(default(TimeSpan));
            var ffmpegWrapperMock = new Mock<IFFmpegWrapper>();
            ffmpegWrapperMock.Setup(m => m.CreateSnapshot(filePath,
                                                                                30,
                                                                                It.IsAny<string>(),
                                                                                tokenSource.Token))
                .ReturnsAsync(conversionResultMock.Object);

            var thumbnailCacheLocationProviderMock = new Mock<IThumbnailCacheLocationProvider>();
            thumbnailCacheLocationProviderMock.Setup(m => m.ProvideLocation(It.IsAny<string>()))
                .Returns(new ThumbnailCacheLocation(string.Empty, string.Empty));

            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapperMock.Object, thumbnailCacheLocationProviderMock.Object, Mock.Of<IThumbnailCreator>());
            var snapshotResult = await provider.CreateSnapshot(filePath, 30, tokenSource.Token)
                .ConfigureAwait(false);

            Assert.That(snapshotResult.Created, Is.EqualTo(true));
        }

        [Test]
        public async Task NotCreateSnapshotIfThumbnailAlreadyExists()
        {
            const string fileName = "new_image";
            const string destination = "/_cache/here/please";
            const string filePath = "/collection/" + fileName + ".jpg";
            const string thumbnailPath = destination + "/" + fileName + "_thumb.png";

            var fileSystem = new FileSystemBuilder()
                .AddFiles(thumbnailPath, new MockFileData(imageBytes))
                .Build();

            var tokenSource = new CancellationTokenSource();

            var thumbnailCacheLocationProviderMock = new Mock<IThumbnailCacheLocationProvider>();
            thumbnailCacheLocationProviderMock.Setup(m => m.ProvideLocation(It.IsAny<string>()))
                .Returns(new ThumbnailCacheLocation(destination, fileName));

            var provider = new VideoInfoProvider(fileSystem, Mock.Of<IFFmpegWrapper>(), thumbnailCacheLocationProviderMock.Object, Mock.Of<IThumbnailCreator>());
            var snapshotResult = await provider.CreateSnapshot(filePath, 30, tokenSource.Token)
                .ConfigureAwait(false);

            Assert.That(snapshotResult.Created, Is.EqualTo(false));
            Assert.That(snapshotResult.SnapshotPath, Is.EqualTo(thumbnailPath));
        }

        [Test, Ignore("Integration Test")]
        public async Task CreateSnapshot_IntegrationTest()
        {
            var tokenSource = new CancellationTokenSource();
            var fileSystem = new FileSystem();
            var ffmpegWrapper = new FFmpegWrapper();
            var filePath = string.Empty;
            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapper, new ThumbnailCacheLocationProvider(new WorkingDirectoryProvider(fileSystem)), Mock.Of<IThumbnailCreator>());
            var snapshotResult = await provider.CreateSnapshot(filePath, 30, tokenSource.Token)
                .ConfigureAwait(false);

            Assert.That(snapshotResult.Created, Is.EqualTo(true));
        }
    }
}
