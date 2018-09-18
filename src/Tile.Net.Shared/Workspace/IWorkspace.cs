using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IWorkspace
    {
        string Name { get; }
        string LayoutName { get; }

        IEnumerable<IWindow> Windows { get; }
        IWindow FocusedWindow { get; }

        IMonitor Monitor { get; set; }
        void AddWindow(IWindow window);
        void RemoveWindow(IWindow window);
        void UpdateWindow(IWindow window);

        void DoLayout(); // mod-n
        void CloseFocusedWindow(); // mod-shift-c
        void NextLayoutEngine(); // mod-space
        void ResetLayout(); // mod-n

        void FocusLastFocusedWindow(); 
        void FocusNextWindow(); // mod-j
        void FocusPreviousWindow(); // mod-k
        void FocusMasterWindow(); // mod-m

        void SwapFocusAndMasterWindow(); // mod-return
        void SwapFocusAndNextWindow(); // mod-shift-j
        void SwapFocusAndPreviousWindow(); // mod-shift-k

        void ShrinkMasterArea(); // mod-h
        void ExpandMasterArea(); // mod-l

        void IncrementNumberOfMasterWindows(); // mod-comma
        void DecrementNumberOfMasterWindows(); // mod-period

        void ForceLayout();

        // mod-t - switch to tiling
        // probably via SetLayoutEngine?

        // non workspace, storing these for a second

        //void ResetLayoutToDefault(); // mod-shift-space

        //void ReloadTileNet(); // mod-q
        //void MoveFocusedWindowToWorkspace(int index); // mod-shift-[1..9]
    }
}
