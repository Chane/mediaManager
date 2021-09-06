using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Providers;

namespace Engine
{
    public class FileHandler
    {
        private readonly IVideoInfoProvider videoInfoProvider;
        private readonly IImageInfoProvider imageInfoProvider;

        public FileHandler(IVideoInfoProvider videoInfoProvider, IImageInfoProvider imageInfoProvider)
        {
            this.videoInfoProvider = videoInfoProvider;
            this.imageInfoProvider = imageInfoProvider;
        }

        public async Task<FileMetaData> HandleAsync(string videoPath, CancellationToken token)
        {
            var fileExtension = videoPath.Split(".").Last().ToLowerInvariant();
            return fileExtension switch
            {
                "jpg" => await this.imageInfoProvider.ProvideAndCreateAsync(videoPath, token),
                "png" => await this.imageInfoProvider.ProvideAndCreateAsync(videoPath, token),
                "mp4" => await this.videoInfoProvider.ProvideAndCreateAsync(videoPath, token),
                _ => null
            };
        }
    }
}