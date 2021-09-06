using System.IO.Abstractions;
using System.Reflection;

namespace Engine.Foundation
{
    public class WorkingDirectoryProvider : IWorkingDirectoryProvider
    {
        private readonly IFileSystem fileSystem;

        public WorkingDirectoryProvider(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }
        public string CurrentExecutingDirectory()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            return this.fileSystem.Path.GetDirectoryName(assemblyLocation);
        }
    }
}