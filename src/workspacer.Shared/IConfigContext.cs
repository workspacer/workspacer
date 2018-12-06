using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface IConfigContext
    {
        IKeybindManager Keybinds { get; }
        IPluginManager Plugins { get; }
        IWorkspaceManager Workspaces { get; }
        ISystemTrayManager SystemTray { get; }
        IWindowsManager Windows { get; }

        IWorkspaceContainer WorkspaceContainer { get; set; }
        IWindowRouter WindowRouter { get; set; }

        Func<ILayoutEngine[]> DefaultLayouts { get; set; }
        void AddLayoutProxy(Func<ILayoutEngine, ILayoutEngine> proxy);
        IEnumerable<ILayoutEngine> ProxyLayouts(IEnumerable<ILayoutEngine> layouts);

        LogLevel ConsoleLogLevel { get; set; }
        LogLevel FileLogLevel { get; set; }
        void ToggleConsoleWindow();

        bool Enabled { get; set; }
        void Quit();
        void Restart();
    }
}
