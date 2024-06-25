using System;
using System.Diagnostics;
using ProjectX.Models;
using ProjectX.Views;

namespace ProjectX.ViewModels.Page;

public static class PythonScriptExecutor
{
    public static string CaptureScannerText(string imagePath)
    {
        var startInfo = new ProcessStartInfo
        {
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = ProjectPathProvider.RecognitionCodeDirectory
        };

        var activator = ServiceLocator.Resolve<IVirtualEnvActivator>();
        var config = AppConfig.Instance;
        var scriptPath = config.GetPythonScript();

        activator.ActivateVirtualEnv(startInfo, imagePath, scriptPath);

        using (var process = Process.Start(startInfo))
        {
            if (process == null)
                throw new Exception("Ошибка процесса");

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
                throw new Exception($"Ошибка: \n{error}");

            return output;
        }
    }
}
