using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public enum LauncherAction
    {
        Quit,
        QuitWithException,
        Restart,
        UpdateHandles,
    }

    public class LauncherResponse
    {
        public LauncherAction Action { get; set; } 
        
        public string ExceptionMessage { get; set; }
        public List<long> ActiveHandles { get; set; }
    }
}
