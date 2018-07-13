using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IWorkspace
    {
        bool IsTemporary { get; }
        IEnumerable<IWindow> Windows { get; }
        IWindow FocusedWindow { get; }

        void Show();
        void Hide();

        void WindowCreated(IWindow window);
        void WindowDestroyed(IWindow window);
        void WindowUpdated(IWindow window);

        void CloseFocusedWindow(); // mod-shift-c
        void NextLayoutEngine(); // mod-space
        void ResetLayout(); // mod-n

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

        // mod-t - switch to tiling
        // probably via SetLayoutEngine?

        // non workspace, storing these for a second

        //void ResetLayoutToDefault(); // mod-shift-space

        //void ReloadTileNet(); // mod-q
        //void MoveFocusedWindowToWorkspace(int index); // mod-shift-[1..9]
    }
}
