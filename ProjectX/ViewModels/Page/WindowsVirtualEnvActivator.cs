using System.Diagnostics;
using ProjectX.Models;

namespace ProjectX.ViewModels.Page;

public class WindowsVirtualEnvActivator : IVirtualEnvActivator
{
    public void ActivateVirtualEnv(ProcessStartInfo startInfo, string imagePath, string scriptPath)
    {
        string venvPath = ProjectPathProvider.VirtualEnvActivationPath;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = $"/c \"{venvPath} && python {scriptPath} \"{imagePath}\"\"";
    }
}
