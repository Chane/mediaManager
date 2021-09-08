using Engine.Foundation;
using Moq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class ThumbnailCacheLocationProviderShould
    {
        [Test]
        public void ProvideLocation()
        {
            const string workingDirectory = "/app/debug";
            const string file = "image1";
            const string filepath = "/collection/images";

            var workingDirectoryProvider = new Mock<IWorkingDirectoryProvider>();
            workingDirectoryProvider.Setup(m => m.CurrentExecutingDirectory()).Returns(workingDirectory);

            var thumbnailCacheLocation = new ThumbnailCacheLocationProvider(workingDirectoryProvider.Object);
            var result = thumbnailCacheLocation.ProvideLocation($"{filepath}/{file}.jpg");

            Assert.That(result.Directory, Is.EqualTo($"{workingDirectory}/_cache{filepath}"));
            Assert.That(result.FileName, Is.EqualTo(file));
        }
    }
}
