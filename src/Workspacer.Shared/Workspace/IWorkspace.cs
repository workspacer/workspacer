using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IWorkspace
    {
        string Name { get; }
        string LayoutName { get; }

        IEnumerable<IWindow> Windows { get; }
        IWindow FocusedWindow { get; }
        IWindow LastFocusedWindow { get; }

        bool IsIndicating { get; set; }

        void AddWindow(IWindow window, bool layout = true);
        void RemoveWindow(IWindow window, bool layout = true);
        void UpdateWindow(IWindow window, WindowUpdateType type, bool layout = true);

        void CloseFocusedWindow(); // mod-shift-c
        void PreviousLayoutEngine(); // mod-space
        void NextLayoutEngine(); // mod-space
        void ResetLayout(); // mod-n

        void FocusLastFocusedWindow(); 
        void FocusNextWindow(); // mod-j
        void FocusPreviousWindow(); // mod-k
        void FocusPrimaryWindow(); // mod-m

        void SwapFocusAndPrimaryWindow(); // mod-return
        void SwapFocusAndNextWindow(); // mod-shift-j
        void SwapFocusAndPreviousWindow(); // mod-shift-k

        void ShrinkPrimaryArea(); // mod-h
        void ExpandPrimaryArea(); // mod-l

        void IncrementNumberOfPrimaryWindows(); // mod-comma
        void DecrementNumberOfPrimaryWindows(); // mod-period

        void ToggleFocusedWindowTiling(); // mod-t

        void DoLayout();

        void SwapWindowToPoint(IWindow window, int x, int y);
        bool IsPointInside(int x, int y);

        // probably via SetLayoutEngine?

        // non workspace, storing these for a second

        //void ResetLayoutToDefault(); // mod-shift-space

        //void ReloadWorkspacer(); // mod-q
        //void MoveFocusedWindowToWorkspace(int index); // mod-shift-[1..9]
    }
}
