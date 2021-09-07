using Engine.Foundation;

namespace Engine
{
    public class ThumbnailCacheLocation
    {
        private readonly IWorkingDirectoryProvider workingDirectoryProvider;

        public ThumbnailCacheLocation(IWorkingDirectoryProvider workingDirectoryProvider)
        {
            this.workingDirectoryProvider = workingDirectoryProvider;
        }

        public string ProvideLocation(string sourceFilePath)
        {
            var workingDirectory = this.workingDirectoryProvider.CurrentExecutingDirectory();
            return $"{workingDirectory}{sourceFilePath}";
        }
    }
}
