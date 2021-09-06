using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Xabe.FFmpeg;

namespace Engine.Tests
{
    [TestFixture]
    public class VideoInfoProviderShould
    {
        // Create Thumbnail
        // Not recreate thumbnail
        // Specify directory for thumbs
        // Read Metadata
        // store thumbs smaller

        [Test]
        public async Task ProvideAndCreate()
        {
            var tokenSource = new CancellationTokenSource();
            var fileSystem = new MockFileSystem();

            var mediaInfoMock = new Mock<IMediaInfo>();
            mediaInfoMock.Setup(m => m.VideoStreams)
                .Returns(new List<IVideoStream> { Mock.Of<IVideoStream>() });

            var conversionResultMock = new Mock<IConversionResult>();
            conversionResultMock.Setup(m => m.Duration)
                .Returns(default(TimeSpan));

            var ffmpegWrapper = new Mock<IFFmpegWrapper>();
            ffmpegWrapper.Setup(m => m.ExtractNthFrame(
                    It.IsAny<IVideoStream>(),
                    It.IsAny<Func<string,string>>(),
                    It.IsAny<int>(),
                    tokenSource.Token))
                .ReturnsAsync(conversionResultMock.Object);
            ffmpegWrapper.Setup(m => m.GetMediaInfoAsync(It.IsAny<string>(), tokenSource.Token))
                .ReturnsAsync(mediaInfoMock.Object);

            var filePath = string.Empty;
            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapper.Object, new WorkingDirectoryProvider(fileSystem));
            var videoDetail = await provider.ProvideAndCreateAsync(filePath, tokenSource.Token).ConfigureAwait(false);

            Assert.That(videoDetail.Created, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateSnapshot()
        {
            var tokenSource = new CancellationTokenSource();
            var filePath = "/a/path";
            var fileSystem = new MockFileSystem();

            var conversionResultMock = new Mock<IConversionResult>();
            var ffmpegWrapperMock = new Mock<IFFmpegWrapper>();
            ffmpegWrapperMock.Setup(m => m.CreateSnapshot(filePath,
                                                                                30,
                                                                                It.IsAny<string>(),
                                                                                tokenSource.Token))
                .ReturnsAsync(conversionResultMock.Object);

            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapperMock.Object, new WorkingDirectoryProvider(fileSystem));
            var videoDetail = await provider.CreateSnapshot(filePath, 30, tokenSource.Token)
                .ConfigureAwait(false);

            Assert.That(videoDetail.Created, Is.EqualTo(true));
        }

        [Test, Ignore("Integration Test")]
        public async Task CreateSnapshot_IntegrationTest()
        {
            var tokenSource = new CancellationTokenSource();
            var fileSystem = new FileSystem();
            var ffmpegWrapper = new FFmpegWrapper();
            var filePath = string.Empty;
            var provider = new VideoInfoProvider(fileSystem, ffmpegWrapper, new WorkingDirectoryProvider(fileSystem));
            var videoDetail = await provider.CreateSnapshot(filePath, 30, tokenSource.Token)
                .ConfigureAwait(false);

            Assert.That(videoDetail.Created, Is.EqualTo(true));
        }
    }
}
