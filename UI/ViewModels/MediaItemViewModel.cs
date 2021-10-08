using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Engine.Models;
using Avalonia.Media.Imaging;
using Engine.Foundation;
using ReactiveUI;

namespace UI.ViewModels
{
    public class MediaItemViewModel : ReactiveObject
    {
        private static readonly List<string> Suffixes = new() { " B", " KB", " MB", " GB", " TB", " PB" };
        private readonly string? filePath;
        private readonly int width;
        private readonly int height;
        private readonly long fileSize;
        private readonly string? thumbnailPath;
        private readonly FileSystem fileSystem;
        private readonly IProcessManager processManager;
        private Bitmap? thumbnail;

        public MediaItemViewModel()
        {
            // TODO: Wire up from DI
            this.fileSystem = new FileSystem();
            this.processManager = new ProcessManager();
        }

        public MediaItemViewModel(FileMetaData fileMetaData) : this()
        {
            (this.filePath, this.thumbnailPath, this.width, this.height, this.fileSize, this.Type) = fileMetaData;
            if (fileMetaData is VideoMetaData videoMetaData)
            {
                this.Duration = videoMetaData.VideoDuration.ToString(@"hh\:mm\:ss");
            }
        }

        public long RawFileSize => this.fileSize;
        public FileType Type { get; }
        public string Dimensions => $"{this.width} x {this.height}";
        public string FileSize => SizeForDisplay(this.fileSize);
        public string? Duration { get; }
        public bool Visible { get; set; }

        public void OnClickCommand()
        {
            var command = $"xdg-open \"{this.filePath}\"";
            command = command.Replace("\"", "\"\"");

            this.processManager.Start(command);
        }

        public Bitmap? Thumbnail
        {
            get => this.thumbnail;
            private set => this.RaiseAndSetIfChanged(ref this.thumbnail, value);
        }

        public async Task LoadCoverAsync()
        {
            await using var imageStream = this.fileSystem.File.OpenRead(this.thumbnailPath);
            if (imageStream != null)
            {
                this.Thumbnail = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 120));
            }
        }

        private static string SizeForDisplay(long number)
        {
            // TODO: Move to Engine
            for (int i = 0; i < Suffixes.Count; i++)
            {
                var temp = number / (int)Math.Pow(1024, i + 1);
                if (temp == 0)
                {
                    return number / (int)Math.Pow(1024, i) + Suffixes[i];
                }
            }

            return number.ToString();
        }
    }
}
