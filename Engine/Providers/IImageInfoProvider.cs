using System.Threading;
using System.Threading.Tasks;
using Engine.Models;

namespace Engine.Providers
{
    public interface IImageInfoProvider
    {
        Task<ImageMetaData> ProvideAndCreateAsync(string imagePath, CancellationToken token);
    }
}