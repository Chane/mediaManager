using System.Threading;
using System.Threading.Tasks;
using Engine.Models;

namespace Engine.Providers
{
    public interface IVideoInfoProvider
    {
        Task<VideoMetaData> ProvideAndCreateAsync(string filePath, CancellationToken token);
    }
}