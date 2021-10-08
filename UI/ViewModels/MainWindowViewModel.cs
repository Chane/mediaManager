using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Engine;
using Engine.Foundation;
using Engine.Models;
using Engine.Providers;
using ReactiveUI;

namespace UI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly DirectoryExplorer directoryExplorer;
        private readonly FileHandler fileHandler;
        private readonly IEnumerable<string> directories;
        private string sourceDirectory = "No folder selected";
        private bool loaded;

        public MainWindowViewModel()
        {
            var fileSystem = new FileSystem();
            var ffmpegWrapper = new FFmpegWrapper();
            var imageWrapper = new ImageWrapper();

            var workingDirectoryProvider = new WorkingDirectoryProvider(fileSystem);
            var thumbnailCacheLocation = new ThumbnailCacheLocationProvider(workingDirectoryProvider);
            var thumbnailCreator = new ThumbnailCreator(fileSystem, thumbnailCacheLocation);
            var imageInfoProvider = new ImageInfoProvider(thumbnailCreator, fileSystem, imageWrapper);
            var videoInfoProvider = new VideoInfoProvider(fileSystem, ffmpegWrapper, thumbnailCacheLocation, thumbnailCreator);

            // Initialize core components with DI
            this.fileHandler = new FileHandler(videoInfoProvider, imageInfoProvider);
            this.directoryExplorer = new DirectoryExplorer(fileSystem);

            const string? collectionDirectoryFromSettings = "";
            this.directories = directoryExplorer.ListDirectories(collectionDirectoryFromSettings);

            this.GenerateTreeView(collectionDirectoryFromSettings);
        }

        public TreeNodeViewModel DirectoryTree { get; } = new() { Directory = "Collection" };

        public ObservableCollection<MediaItemViewModel> Items { get; } = new ();

        public bool ShowImages { get; set; } = true;

        public bool ShowVideo { get; set; } = true;

        public string SourceDirectory
        {
            get => this.sourceDirectory;
            set => this.RaiseAndSetIfChanged(ref this.sourceDirectory, value);
        }

        public bool Loaded
        {
            get => this.loaded;
            set => this.RaiseAndSetIfChanged(ref this.loaded, value);
        }

        public async Task Refresh()
        {
            this.Loaded = false;
            this.Items.Clear();
            var cancellationTokenSource = new CancellationTokenSource();

            var source = this.directories.Where(d => d.Contains(this.SourceDirectory));

            foreach (var directory in source)
            {
                var files = this.directoryExplorer.ListFiles(directory);
                foreach (var file in files)
                {
                    var item = await this.fileHandler.HandleAsync(file, cancellationTokenSource.Token);
                    if (item is null)
                    {
                        Debug.WriteLine($"Item is null {file}");
                        continue;
                    }

                    var vm = new MediaItemViewModel(item);
                    switch (vm.Type)
                    {
                        case FileType.Image when this.ShowImages:
                        case FileType.Video when this.ShowVideo:
                            vm.Visible = true;
                            break;
                    }

                    await vm.LoadCoverAsync();
                    this.Items.Add(vm);
                }
            }

            this.Loaded = true;
        }

        public void ApplyFilter()
        {
            var images = this.Items.Where(i => i.Type == FileType.Image).ToList();
            images.ForEach(i => i.Visible = this.ShowImages);

            var videos = this.Items.Where(i => i.Type == FileType.Video).ToList();
            videos.ForEach(i => i.Visible = this.ShowVideo);

            this.Items.Clear();
            this.Items.AddRange(images);
            this.Items.AddRange(videos);
        }

        private void GenerateTreeView(string rootDirectory)
        {
            var topLevelDirectories= this.directories
                .Select(s => s.Replace(rootDirectory, string.Empty, StringComparison.OrdinalIgnoreCase))
                .Where(s => s.IndexOf("/", StringComparison.OrdinalIgnoreCase) == -1 &&
                            !string.IsNullOrWhiteSpace(s))
                .OrderBy(d => d)
                .ToList();

            foreach (var directory in topLevelDirectories)
            {
                var node = ProcessInnerChild($"{rootDirectory}{directory}/", directory);
                this.DirectoryTree.Directories.Add(node);
            }
        }

        private TreeNodeViewModel ProcessInnerChild(string fullPath, string directory)
        {
            var node = new TreeNodeViewModel { Directory = directory };
            foreach (var childPath in this.directoryExplorer.ListDirectories(fullPath))
            {
                if (childPath == fullPath) continue;

                var childDirectory = childPath.Replace(fullPath, string.Empty, StringComparison.OrdinalIgnoreCase);
                if (childDirectory.IndexOf("/", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    var childNode = this.ProcessInnerChild($"{childPath}/", childDirectory);
                    node.Directories.Add(childNode);
                }
            }

            return node;
        }
    }
}
