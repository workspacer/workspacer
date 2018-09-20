using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public delegate void WorkspaceUpdatedDelegate();
    public delegate void FocusedMonitorUpdatedDelegate();

    public interface IWorkspaceManager
    {
        IEnumerable<IWorkspace> Workspaces { get; }
        IEnumerable<IMonitor> Monitors { get; }
        Func<IWindow, IWorkspace> WorkspaceSelectorFunc { get; set; }
        Func<IWindow, bool> WindowFilterFunc { get; set; }
        void AddWorkspace(string name, ILayoutEngine[] layouts);
        IWorkspace FocusedWorkspace { get; }
        IMonitor FocusedMonitor { get; }
        void SwitchToWorkspace(int index);
        void SwitchFocusedMonitor(int index);
        void SwitchFocusedMonitorToMouseLocation();
        void MoveFocusedWindowToWorkspace(int index);
        void MoveFocusedWindowToMonitor(int index);
        IMonitor GetMonitorForWorkspace(IWorkspace workspace);
        IWorkspace GetWorkspaceForMonitor(IMonitor monitor);

        IWorkspace this[int index] { get; }
        IWorkspace this[string name] { get; }

        event WorkspaceUpdatedDelegate WorkspaceUpdated;
        event FocusedMonitorUpdatedDelegate FocusedMonitorUpdated;
    }
}
