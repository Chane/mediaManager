using System.Threading;
using System.Threading.Tasks;
using Engine.Providers;
using Moq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class FileHandlerShould
    {
        private Mock<IVideoInfoProvider> videoInfoProviderMock;
        private Mock<IImageInfoProvider> imageInfoProviderMock;

        [SetUp]
        public void Setup()
        {
            this.videoInfoProviderMock = new Mock<IVideoInfoProvider>();
            this.imageInfoProviderMock = new Mock<IImageInfoProvider>();
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

        private FileHandler Create() => new FileHandler(this.videoInfoProviderMock.Object, this.imageInfoProviderMock.Object);
    }
}