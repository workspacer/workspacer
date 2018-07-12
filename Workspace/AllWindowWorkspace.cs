using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Tile.Net
{
    public class AllWindowWorkspace : IWorkspace
    {
        private List<IWindow> _windows;
        public IEnumerable<IWindow> Windows => _windows;

        public void WindowCreated(IWindow window)
        {
            _windows.Add(window);
            DoLayout();
        }

        public void WindowDestroyed(IWindow window)
        {
            _windows.Remove(window);
            DoLayout();
        }

        public void WindowUpdated(IWindow window)
        {
            DoLayout();
        }

        public void ResetLayout()
        {
            _layoutEngine.ResetMasterArea();
            DoLayout();
        }

        public void FocusNextWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            for (var i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                if (window.IsFocused)
                {
                    if (i + 1 == windows.Count)
                    {
                        windows[0].IsFocused = true;
                    }
                    else
                    {
                        windows[i + 1].IsFocused = true;
                    }
                    break;
                }
            }
        }

        public void FocusPreviousWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            for (var i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                if (window.IsFocused)
                {
                    if (i == 0)
                    {
                        windows[windows.Count - 1].IsFocused = true;
                    }
                    else
                    {
                        windows[i - 1].IsFocused = true;
                    }
                    break;
                }
            }
        }

        public void FocusMasterWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            if (windows.Count > 0)
            {
                windows[0].IsFocused = true;
            }
        }

        public void SwapFocusAndMasterWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            if (windows.Count > 1)
            {
                var master = windows[0];
                var focus = windows.FirstOrDefault(w => w.IsFocused);

                if (focus != null)
                {
                    SwapWindows(master, focus);
                }
            }
        }

        public void SwapFocusAndNextWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            for (var i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                if (window.IsFocused)
                {
                    if (i + 1 == windows.Count)
                    {
                        SwapWindows(window, windows[0]);
                    }
                    else
                    {
                        SwapWindows(window, windows[i + 1]);
                    }
                    break;
                }
            }
        }

        public void SwapFocusAndPreviousWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            for (var i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                if (window.IsFocused)
                {
                    if (i == 0)
                    {
                        SwapWindows(window, windows[windows.Count - 1]);
                    }
                    else
                    {
                        SwapWindows(window, windows[i - 1]);
                    }
                    break;
                }
            }
        }

        public void ShrinkMasterArea()
        {
            _layoutEngine.ShrinkMasterArea();
            DoLayout();
        }
        public void ExpandMasterArea()
        {
            _layoutEngine.ExpandMasterArea();
            DoLayout();
        }

        private ILayoutEngine _layoutEngine;
        private WinEventDelegate _moveDelegate;

        public AllWindowWorkspace(ILayoutEngine layoutEngine)
        {
            _layoutEngine = layoutEngine;
            _windows = new List<IWindow>();
        }

        public void DoLayout()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            var bounds = Screen.PrimaryScreen.WorkingArea;
            var locations = _layoutEngine.CalcLayout(windows.Count(), bounds.Width, bounds.Height).ToArray();

            using (var handle = WindowsDesktopManager.Instance.DeferWindowsPos(windows.Count))
            {
                for (var i = 0; i < locations.Length; i++)
                {
                    var window = windows[i];
                    var loc = locations[i];

                    if (window.IsMaximized)
                    {
                        window.ShowNormal();
                    }

                    handle.DeferWindowPos(window, loc.X, loc.Y, loc.Width, loc.Height);
                }
            }
        }

        private void SwapWindows(IWindow left, IWindow right)
        {
            var leftIdx = _windows.FindIndex(w => w == left);
            var rightIdx = _windows.FindIndex(w => w == right);

            _windows[leftIdx] = right;
            _windows[rightIdx] = left;

            DoLayout();
        }
    }
}
