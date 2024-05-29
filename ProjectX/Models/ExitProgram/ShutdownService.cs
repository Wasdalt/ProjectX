using System;

namespace ProjectX.Models.ExitProgram;

public class ShutdownService : IShutdownService
{
    public void Shutdown()
    {
        Environment.Exit(0);
    }
}
