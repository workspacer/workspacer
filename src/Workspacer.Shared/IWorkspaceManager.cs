using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public delegate void WorkspaceUpdatedDelegate();
    public delegate void FocusedMonitorUpdatedDelegate();
    public delegate void WindowAddedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowUpdatedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowRemovedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowMovedDelegate(IWindow window, IWorkspace oldWorkspace, IWorkspace newWorkspace);

    public interface IWorkspaceManager
    {
        IEnumerable<IMonitor> Monitors { get; }

        IWorkspace FocusedWorkspace { get; }
        IMonitor FocusedMonitor { get; }

        void SwitchToWindow(IWindow window);
        void SwitchToWorkspace(int index);
        void SwitchToWorkspace(IWorkspace workspace);
        void SwitchMonitorToWorkspace(int monitorIndex, int workspaceIndex);
        void SwitchToNextWorkspace();
        void SwitchToPreviousWorkspace();
        void SwitchFocusedMonitor(int index);
        void SwitchFocusedMonitorToMouseLocation();
        void MoveFocusedWindowToWorkspace(int index);
        void MoveFocusedWindowToMonitor(int index);
        void MoveAllWindows(IWorkspace source, IWorkspace dest);

        void ForceWorkspaceUpdate();

        event WorkspaceUpdatedDelegate WorkspaceUpdated;
        event WindowAddedDelegate WindowAdded;
        event WindowUpdatedDelegate WindowUpdated;
        event WindowRemovedDelegate WindowRemoved;
        event WindowMovedDelegate WindowMoved;
        event FocusedMonitorUpdatedDelegate FocusedMonitorUpdated;
    }
}
