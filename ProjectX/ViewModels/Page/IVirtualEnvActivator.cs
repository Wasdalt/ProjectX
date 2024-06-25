using System.Diagnostics;

namespace ProjectX.ViewModels.Page;

public interface IVirtualEnvActivator
{
    void ActivateVirtualEnv(ProcessStartInfo startInfo, string imagePath, string scriptPath);
}