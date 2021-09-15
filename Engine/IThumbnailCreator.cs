using System.Threading;
using System.Threading.Tasks;
using Engine.Models;

namespace Engine
{
    public interface IThumbnailCreator
    {
        Task<ThumbnailResult> CreateAsync(string filePath, CancellationToken token);
        Task<ThumbnailResult> CreateAsync(string filePath, string destinationPath, CancellationToken token);
    }
}