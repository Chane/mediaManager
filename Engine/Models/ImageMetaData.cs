namespace Engine.Models
{
    public record ImageMetaData(string FilePath, string ThumbnailPath, int Width, int Height, long FileSize, string Resolution)
        : FileMetaData(FilePath, ThumbnailPath, Width, Height, FileSize, FileType.Image);
}
