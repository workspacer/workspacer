using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    /// <summary>
    /// IWorkspace provides a common interface for workspace-related operations.
    /// workspaces logically contain a set of windows, and allow callers to interact with the 
    /// windows via a set of methods, and all the organization of the windows via layout engines
    /// </summary>
    public interface IWorkspace
    {
        /// <summary>
        /// name of the workspace
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// name of the currently active layout
        /// </summary>
        string LayoutName { get; }

        /// <summary>
        /// set of windows that are contained within the workspace
        /// </summary>
        IEnumerable<IWindow> Windows { get; }

        /// <summary>
        /// set of windows that are contained within the workspace and which workspacer can layout.
        /// </summary>
        IList<IWindow> ManagedWindows { get; }

        /// <summary>
        /// currently focused window in the workspace, or null if there is no focused window in the workspace
        /// </summary>
        IWindow FocusedWindow { get; }

        /// <summary>
        /// the last focused window in the workspace
        /// </summary>
        IWindow LastFocusedWindow { get; }

        /// <summary>
        /// whether the workspace is currently indicating (flashing)
        /// </summary>
        bool IsIndicating { get; set; }

        void AddWindow(IWindow window, bool layout = true);
        void RemoveWindow(IWindow window, bool layout = true);
        void UpdateWindow(IWindow window, WindowUpdateType type, bool layout = true);

        /// <summary>
        /// close the currently focused window
        /// </summary>
        void CloseFocusedWindow(); // mod-shift-c

        /// <summary>
        /// rotate to the previous layout
        /// </summary>
        void PreviousLayoutEngine(); // mod-space

        /// <summary>
        /// rotate to the next layout
        /// </summary>
        void NextLayoutEngine(); // mod-space

        /// <summary>
        ///  reset the active layout
        /// </summary>
        void ResetLayout(); // mod-n

        /// <summary>
        /// focus the last focused window
        /// </summary>
        void FocusLastFocusedWindow(); 

        /// <summary>
        /// rotate focus to the next window
        /// </summary>
        void FocusNextWindow(); // mod-j

        /// <summary>
        /// rotate focus to the previous window
        /// </summary>
        void FocusPreviousWindow(); // mod-k

        /// <summary>
        /// focus the primary window
        /// </summary>
        void FocusPrimaryWindow(); // mod-m

        /// <summary>
        /// swap the focus and primary windows
        /// </summary>
        void SwapFocusAndPrimaryWindow(); // mod-return

        /// <summary>
        /// swap the focus and next windows
        /// </summary>
        void SwapFocusAndNextWindow(); // mod-shift-j

        /// <summary>
        /// swap the focus and previous windows
        /// </summary>
        void SwapFocusAndPreviousWindow(); // mod-shift-k

        /// <summary>
        /// shrink the primary area of the active layout
        /// </summary>
        void ShrinkPrimaryArea(); // mod-h

        /// <summary>
        /// expand the primary area of the active layout
        /// </summary>
        void ExpandPrimaryArea(); // mod-l

        /// <summary>
        /// increase the number of primary windows in the active layout
        /// </summary>
        void IncrementNumberOfPrimaryWindows(); // mod-comma

        /// <summary>
        /// decrease the number of primary windows in the active layout
        /// </summary>
        void DecrementNumberOfPrimaryWindows(); // mod-period

        /// <summary>
        /// force a layout of the workspace
        /// </summary>
        void DoLayout();

        /// <summary>
        /// swap the specified window to a (x,y) point in the workspace
        /// </summary>
        /// <param name="window">window to swap</param>
        /// <param name="x">x coordinate of the point</param>
        /// <param name="y">y coordinate of the point</param>
        void SwapWindowToPoint(IWindow window, int x, int y);

        /// <summary>
        /// check if the given point is in the workspace
        /// </summary>
        /// <param name="x">x coordinate of the point</param>
        /// <param name="y">y coordinate of the point</param>
        /// <returns>true if the point is inside the workspace, false otherwise</returns>
        bool IsPointInside(int x, int y);
    }
}
