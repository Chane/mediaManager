namespace Engine
{
    public interface IThumbnailCacheLocation
    {
        string ProvideLocation(string sourceFilePath);
    }
}
