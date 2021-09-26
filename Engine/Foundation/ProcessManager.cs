using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Foundation
{
    [ExcludeFromCodeCoverage]
    public class ProcessManager : IProcessManager
    {
        public void Start(string command)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
            };

            proc.Start();
        }
    }
}
