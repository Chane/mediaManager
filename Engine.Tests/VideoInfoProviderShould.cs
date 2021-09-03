using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

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

        [Test, Ignore("WIP")]
        public async Task ProvideAndCreate()
        {
            var tokenSource = new CancellationTokenSource();
            var fileSystem = new FileSystem();
            var filePath = string.Empty;
            var provider = new VideoInfoProvider(fileSystem);
            var videoDetail = await provider.ProvideAndCreateAsync(filePath, tokenSource.Token).ConfigureAwait(false);

            Assert.That(videoDetail.Created, Is.EqualTo(true));
        }
    }
}