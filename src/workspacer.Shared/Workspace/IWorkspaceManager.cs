namespace workspacer
{
    public delegate void WorkspaceUpdatedDelegate();
    public delegate void FocusedMonitorUpdatedDelegate();
    public delegate void WindowFocusedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowAddedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowUpdatedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowRemovedDelegate(IWindow window, IWorkspace workspace);
    public delegate void WindowMovedDelegate(IWindow window, IWorkspace workspace, IWorkspace newWorkspace = null);

    public interface IWorkspaceManager
    {
        /// <summary>
        /// Currently focused workspace, or null if there is no focused workspace
        /// </summary>
        IWorkspace FocusedWorkspace { get; }

        /// <summary>
        /// Focus window and it's workspace
        /// </summary>
        /// <param name="window">window instance</param>
        void SwitchToWindow(IWindow window);

        /// <summary>
        /// Switch to workspace
        /// </summary>
        /// <param name="index">index of workspace</param>
        void SwitchToWorkspace(int index);

        /// <summary>
        /// Switch to workspace
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void SwitchToWorkspace(IWorkspace workspace);

        /// <summary>
        /// Switch to previously focused workspace
        /// </summary>
        void SwitchToLastFocusedWorkspace();

        /// <summary>
        /// Switch monitor to workspace
        /// </summary>
        /// <param name="monitorIndex">index of monitor</param>
        /// <param name="workspaceIndex">index of workspace</param>
        void SwitchMonitorToWorkspace(int monitorIndex, int workspaceIndex);

        /// <summary>
        /// Switch to next workspace
        /// </summary>
        void SwitchToNextWorkspace();

        /// <summary>
        /// Switch to previous workspace
        /// </summary>
        void SwitchToPreviousWorkspace();

        /// <summary>
        /// Switch focus to monitor
        /// </summary>
        /// <param name="index">index of monitor</param>
        void SwitchFocusedMonitor(int index);

        /// <summary>
        /// Switch focus to next monitor
        /// </summary>
        void SwitchFocusToNextMonitor();

        /// <summary>
        /// Switch focus to previous monitor
        /// </summary>
        void SwitchFocusToPreviousMonitor();

        /// <summary>
        /// Switch focused monitor to mouse location
        /// </summary>
        void SwitchFocusedMonitorToMouseLocation();

        /// <summary>
        /// Move focused monitor to workspace
        /// </summary>
        /// <param name="index">workspace index</param>
        void MoveFocusedWindowToWorkspace(int index);

        /// <summary>
        /// Move to specific workspace
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void MoveFocusedWindowAndSwitchToNextWorkspace();

        /// <summary>
        /// Move to specific workspace
        /// </summary>
        /// <param name="workspace">workspace to focus</param>
        void MoveFocusedWindowAndSwitchToPreviousWorkspace();

        /// <summary>
        /// Move to specific workspace
        /// </summary>
        /// <param name="workspace">workspace to focus</param>
        void MoveFocusedWindowToMonitor(int index);

        /// <summary>
        /// Move all windows in workspace to destination workspace
        /// </summary>
        /// <param name="source">source workspace</param>
        /// <param name="dest">destination workspace</param>
        void MoveAllWindows(IWorkspace source, IWorkspace dest);

        /// <summary>
        /// Move focused window in focused workspace to workspace in next monitor
        /// </summary>
        void MoveFocusedWindowToNextMonitor();

        /// <summary>
        /// Move focused window in focused workspace to workspace in previous monitor
        /// </summary>
        void MoveFocusedWindowToPreviousMonitor();

        /// <summary>
        /// Close the currently focused window
        /// </summary>
        /// <param name="workspace">workspace instance/param>
        void CloseFocusedWindow(IWorkspace workspace);

        /// <summary>
        /// Focus the last focused window
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void FocusLastFocusedWindow(IWorkspace workspace);

        /// <summary>
        /// Rotate focus to the next window
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void FocusNextWindow(IWorkspace workspace);

        /// <summary>
        /// Rotate focus to the previous window
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void FocusPreviousWindow(IWorkspace workspace);

        /// <summary>
        /// Focus the primary window
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void FocusPrimaryWindow(IWorkspace workspace);

        /// <summary>
        /// Swap the focus and primary windows
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void SwapFocusAndPrimaryWindow(IWorkspace workspace);

        /// <summary>
        /// Swap the focus and next windows
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void SwapFocusAndNextWindow(IWorkspace workspace);

        /// <summary>
        /// Swap the focus and previous windows
        /// </summary>
        /// <param name="workspace">workspace instance</param>
        void SwapFocusAndPreviousWindow(IWorkspace workspace);

        void ForceWorkspaceUpdate();

        event WorkspaceUpdatedDelegate WorkspaceUpdated;
        event WindowFocusedDelegate WindowFocused;
        event WindowAddedDelegate WindowAdded;
        event WindowUpdatedDelegate WindowUpdated;
        event WindowRemovedDelegate WindowRemoved;
        event WindowMovedDelegate WindowMoved;
        event FocusedMonitorUpdatedDelegate FocusedMonitorUpdated;
    }
}
