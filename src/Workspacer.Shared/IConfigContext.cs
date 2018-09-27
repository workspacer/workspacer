using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IConfigContext
    {
        IKeybindManager Keybinds { get; }
        IPluginManager Plugins { get; }
        IWorkspaceManager Workspaces { get; }
        ISystemTrayManager SystemTray { get; }
        IWindowsManager Windows { get; }

        bool Enabled { get; set; }
        void Quit();
        void Restart();
    }
}
