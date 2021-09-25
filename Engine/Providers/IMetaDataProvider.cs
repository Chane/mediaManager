using System.Threading;
using System.Threading.Tasks;
using Engine.Models;

namespace Engine.Providers
{
    public interface IMetaDataProvider<T> where T : FileMetaData
    {
        Task<T> ProvideAndCreateAsync(string filePath, CancellationToken token);
    }
}
