using System.Collections.Generic;

namespace workspacer
{
    public enum LauncherAction
    {
        Quit,
        QuitWithException,
        Restart,
        RestartWithMessage,
        UpdateHandles,
        ToggleConsole,
        Log,
    }

    public class LauncherResponse
    {
        public LauncherAction Action { get; set; } 
        
        public string Message { get; set; }
        public List<long> ActiveHandles { get; set; }
    }
}
