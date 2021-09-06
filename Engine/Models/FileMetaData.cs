namespace Engine.Models
{
    public record FileMetaData(string FilePath, string ThumbnailPath, int Width, int Height, long FileSize, FileType FileType = FileType.NotSupported);
}
