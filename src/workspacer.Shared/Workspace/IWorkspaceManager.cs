using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public delegate void WorkspaceUpdatedDelegate();
    public delegate void FocusedMonitorUpdatedDelegate();
    public delegate void WindowAddedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowUpdatedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowRemovedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowMovedDelegate(IWindow window, IWorkspace oldWorkspace, IWorkspace newWorkspace);

    public interface IWorkspaceManager
    {
        IWorkspace FocusedWorkspace { get; }

        void SwitchToWindow(IWindow window);
        void SwitchToWorkspace(int index);
        void SwitchToWorkspace(IWorkspace workspace);
        void SwitchToLastFocusedWorkspace();
        void SwitchMonitorToWorkspace(int monitorIndex, int workspaceIndex);
        void SwitchToNextWorkspace();
        void SwitchToPreviousWorkspace();
        void SwitchFocusedMonitor(int index);
        void SwitchFocusToNextMonitor();
        void SwitchFocusToPreviousMonitor();
        void SwitchFocusedMonitorToMouseLocation();
        void MoveFocusedWindowToWorkspace(int index);
        void MoveFocusedWindowToMonitor(int index);
        void MoveAllWindows(IWorkspace source, IWorkspace dest);
        void MoveFocusedWindowToNextMonitor();
        void MoveFocusedWindowToPreviousMonitor();

        void ForceWorkspaceUpdate();

        event WorkspaceUpdatedDelegate WorkspaceUpdated;
        event WindowAddedDelegate WindowAdded;
        event WindowUpdatedDelegate WindowUpdated;
        event WindowRemovedDelegate WindowRemoved;
        event WindowMovedDelegate WindowMoved;
        event FocusedMonitorUpdatedDelegate FocusedMonitorUpdated;
    }
}
