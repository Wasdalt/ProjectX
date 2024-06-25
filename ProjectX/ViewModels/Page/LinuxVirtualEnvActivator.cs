using System.Diagnostics;
using ProjectX.Models;

namespace ProjectX.ViewModels.Page;

public class LinuxVirtualEnvActivator : IVirtualEnvActivator
{
    public void ActivateVirtualEnv(ProcessStartInfo startInfo, string imagePath, string scriptPath)
    {
        string venvPath = ProjectPathProvider.VirtualEnvActivationPath;
        startInfo.FileName = "bash";
        startInfo.Arguments = $"-c \"source {venvPath} && python3 {scriptPath} \"{imagePath}\"\"";
    }
}