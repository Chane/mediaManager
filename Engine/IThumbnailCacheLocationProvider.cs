using Engine.Models;

namespace Engine
{
    public interface IThumbnailCacheLocationProvider
    {
        ThumbnailCacheLocation ProvideLocation(string sourceFilePath);
    }
}
