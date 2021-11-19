using System.Collections.Generic;

namespace workspacer
{
    /// <summary>
    /// IWorkspace provides a common interface for workspace-related operations.
    /// Workspaces logically contain a set of windows, and allow callers to interact with the 
    /// windows via a set of methods, and all the organization of the windows via layout engines
    /// </summary>
    public interface IWorkspace
    {
        /// <summary>
        /// Name of the workspace
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Name of the currently active layout
        /// </summary>
        string LayoutName { get; }

        /// <summary>
        /// Set of windows that are contained within the workspace
        /// </summary>
        IEnumerable<IWindow> Windows { get; }

        /// <summary>
        /// Set of windows that are contained within the workspace and which workspacer can layout.
        /// </summary>
        IList<IWindow> ManagedWindows { get; }

        /// <summary>
        /// Currently focused window in the workspace, or null if there is no focused window in the workspace
        /// </summary>
        IWindow FocusedWindow { get; }

        /// <summary>
        /// The last focused window in the workspace
        /// </summary>
        IWindow LastFocusedWindow { get; }

        /// <summary>
        /// Whether the workspace is currently indicating (flashing)
        /// </summary>
        bool IsIndicating { get; set; }

        /// <summary>
        /// Add window to workspace
        /// </summary>
        /// <param name="window">window to add to workspace</param>
        /// <param name="layout">force layout of the workspace/param>
        void AddWindow(IWindow window, bool layout = true);

        /// <summary>
        /// Remove window from workspace
        /// </summary>
        /// <param name="window">window to add to workspace</param>
        /// <param name="layout">force layout of the workspace/param>
        void RemoveWindow(IWindow window, bool layout = true);

        /// <summary>
        /// Update window in workspace
        /// </summary>
        /// <param name="window">window to add to workspace</param>
        /// <param name="type">type of update</param>
        /// <param name="layout">force layout of the workspace/param>
        void UpdateWindow(IWindow window, WindowUpdateType type, bool layout = true);

        /// <summary>
        /// Rotate to the previous layout
        /// </summary>
        void PreviousLayoutEngine(); // mod-space

        /// <summary>
        /// Rotate to the next layout
        /// </summary>
        void NextLayoutEngine(); // mod-space

        /// <summary>
        /// Reset the active layout
        /// </summary>
        void ResetLayout(); // mod-n

        /// <summary>
        /// Shrink the primary area of the active layout
        /// </summary>
        void ShrinkPrimaryArea(); // mod-h

        /// <summary>
        /// Expand the primary area of the active layout
        /// </summary>
        void ExpandPrimaryArea(); // mod-l

        /// <summary>
        /// Increase the number of primary windows in the active layout
        /// </summary>
        void IncrementNumberOfPrimaryWindows(); // mod-comma

        /// <summary>
        /// Decrease the number of primary windows in the active layout
        /// </summary>
        void DecrementNumberOfPrimaryWindows(); // mod-period
        /// <summary>
        /// Force a layout of the workspace
        /// </summary>
        void DoLayout();

        /// <summary>
        /// Swap the specified window to a (x,y) point in the workspace
        /// </summary>
        /// <param name="window">window to swap</param>
        /// <param name="x">x coordinate of the point</param>
        /// <param name="y">y coordinate of the point</param>
        void SwapWindowToPoint(IWindow window, int x, int y);

        /// <summary>
        /// Swap two windows in the workspace
        /// </summary>
        /// <param name="left">first window to swap</param>
        /// <param name="right">second window to swap</param>
        void SwapWindows(IWindow left, IWindow right);

        /// <summary>
        /// Check if the given point is in the workspace
        /// </summary>
        /// <param name="x">x coordinate of the point</param>
        /// <param name="y">y coordinate of the point</param>
        /// <returns>true if the point is inside the workspace, false otherwise</returns>
        bool IsPointInside(int x, int y);
    }
}
