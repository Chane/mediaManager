using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Engine.Foundation;
using Engine.Providers;
using Moq;
using NUnit.Framework;
using Xabe.FFmpeg;

namespace Engine.Tests
{
    [TestFixture]
    public class VideoInfoProviderShould
    {
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

            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapperMock.Object, new WorkingDirectoryProvider(fileSystem), Mock.Of<IThumbnailCreator>());
            var videoDetail = await provider.ProvideAndCreateAsync(filePath, tokenSource.Token).ConfigureAwait(false);

            Assert.That(videoDetail, Is.Not.Null);
        }

        [Test]
        public async Task CreateSnapshot()
        {
            var tokenSource = new CancellationTokenSource();
            var filePath = "/a/path";
            var fileSystem = new MockFileSystem();

            var conversionResultMock = new Mock<IConversionResult>();
            conversionResultMock.Setup(m => m.Duration).Returns(default(TimeSpan));
            var ffmpegWrapperMock = new Mock<IFFmpegWrapper>();
            ffmpegWrapperMock.Setup(m => m.CreateSnapshot(filePath,
                                                                                30,
                                                                                It.IsAny<string>(),
                                                                                tokenSource.Token))
                .ReturnsAsync(conversionResultMock.Object);

            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapperMock.Object, new WorkingDirectoryProvider(fileSystem), Mock.Of<IThumbnailCreator>());
            var snapshotResult = await provider.CreateSnapshot(filePath, 30, tokenSource.Token)
                .ConfigureAwait(false);

            Assert.That(snapshotResult.Created, Is.EqualTo(true));
        }

        [Test, Ignore("Integration Test")]
        public async Task CreateSnapshot_IntegrationTest()
        {
            var tokenSource = new CancellationTokenSource();
            var fileSystem = new FileSystem();
            var ffmpegWrapper = new FFmpegWrapper();
            var filePath = string.Empty;
            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapper, new WorkingDirectoryProvider(fileSystem), Mock.Of<IThumbnailCreator>());
            var snapshotResult = await provider.CreateSnapshot(filePath, 30, tokenSource.Token)
                .ConfigureAwait(false);

            Assert.That(snapshotResult.Created, Is.EqualTo(true));
        }
    }
}
