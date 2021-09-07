using Engine.Foundation;
using Moq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class ThumbnailCacheLocationShould
    {
        [Test]
        public void ProvideLocation()
        {
            const string workingDirectory = "/app/debug";
            const string filepath = "/collection/images/image1.jpg";

            var workingDirectoryProvider = new Mock<IWorkingDirectoryProvider>();
            workingDirectoryProvider.Setup(m => m.CurrentExecutingDirectory()).Returns(workingDirectory);

            var thumbnailCacheLocation = new ThumbnailCacheLocation(workingDirectoryProvider.Object);
            var result = thumbnailCacheLocation.ProvideLocation(filepath);

            Assert.That(result, Is.EqualTo($"{workingDirectory}{filepath}"));
        }
    }
}
