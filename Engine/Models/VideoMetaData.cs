using System;

namespace Engine.Models
{
    public record VideoMetaData(string FilePath, string ThumbnailPath, int Width, int Height, long FileSize, TimeSpan VideoDuration)
        : FileMetaData(FilePath, ThumbnailPath, Width, Height, FileSize, FileType.Video);
}
