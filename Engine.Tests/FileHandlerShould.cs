using System.Threading;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Providers;
using Moq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class FileHandlerShould
    {
        private Mock<IMetaDataProvider<VideoMetaData>> videoInfoProviderMock;
        private Mock<IMetaDataProvider<ImageMetaData>> imageInfoProviderMock;

        [SetUp]
        public void Setup()
        {
            this.videoInfoProviderMock = new Mock<IMetaDataProvider<VideoMetaData>>();
            this.imageInfoProviderMock = new Mock<IMetaDataProvider<ImageMetaData>>();
        }

        [TestCase("/video.mp4")]
        public async Task PassVideoToVideoInfoProvider(string filePath)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var handler = this.Create();
            await handler.HandleAsync(filePath, cancellationTokenSource.Token);

            this.videoInfoProviderMock.Verify(v => v.ProvideAndCreateAsync(filePath, cancellationTokenSource.Token));
        }

        [TestCase("/image.jpg")]
        [TestCase("/image.png")]
        public async Task PassImageToImageInfoProvider(string filePath)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var handler = this.Create();
            await handler.HandleAsync(filePath, cancellationTokenSource.Token);

            this.imageInfoProviderMock.Verify(v => v.ProvideAndCreateAsync(filePath, cancellationTokenSource.Token));
        }

        [Test]
        public async Task ReturnNull()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var handler = this.Create();
            var result = await handler.HandleAsync("/blah.text", cancellationTokenSource.Token);

            Assert.That(result, Is.Null);
        }

        private FileHandler Create() => new FileHandler(this.videoInfoProviderMock.Object, this.imageInfoProviderMock.Object);
    }
}