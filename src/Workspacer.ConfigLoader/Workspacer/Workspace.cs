using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Workspacer
{
    public class Workspace : IWorkspace
    {
        private static Logger Logger = Logger.Create();

        public IEnumerable<IWindow> Windows => _windows;
        public IWindow FocusedWindow => _windows.FirstOrDefault(w => w.IsFocused);
        public IWindow LastFocusedWindow => _lastFocused;
        public string Name { get; }
        public string LayoutName => _layoutEngines[_layoutIndex].Name;
        public bool IsIndicating { get; set; }

        private IConfigContext _context;
        private List<IWindow> _windows;
        private ILayoutEngine[] _layoutEngines;
        private int _layoutIndex;
        private IWindow _lastFocused;

        private Dictionary<IWindow, bool> _floating;

        public Workspace(IConfigContext context, string name, ILayoutEngine[] layoutEngines)
        {
            _context = context;
            _layoutEngines = layoutEngines;
            _layoutIndex = 0;
            _windows = new List<IWindow>();

            _lastFocused = null;
            _floating = new Dictionary<IWindow, bool>();
            Name = name;
        }

        public void AddWindow(IWindow window)
        {
            if (_lastFocused == null && window.IsFocused)
            {
                _lastFocused = window;
            }

            _windows.Add(window);
            DoLayout();
        }

        public void RemoveWindow(IWindow window)
        {
            if (_lastFocused == window)
            {
                var windows = _windows.Where(w => w.CanLayout).ToList();
                var next = windows.Count > 1 ? windows[(windows.IndexOf(window) + 1) % windows.Count] : null;
                _lastFocused = next;
            }

            _windows.Remove(window);
            DoLayout();
        }

        public void UpdateWindow(IWindow window, WindowUpdateType type)
        {
            if (type == WindowUpdateType.Foreground)
                _lastFocused = window;

            DoLayout();
        }

        public void CloseFocusedWindow()
        {
            var window = this.Windows.FirstOrDefault(w => w.CanLayout && w.IsFocused);
            window?.Close();
        }

        public void PreviousLayoutEngine()
        {
            if (_layoutIndex == 0)
            {
                _layoutIndex = _layoutEngines.Length - 1;
            }
            else
            {
                _layoutIndex--;
            }
            DoLayout();
        }

        public void NextLayoutEngine()
        {
            if (_layoutIndex + 1 == _layoutEngines.Length)
            {
                _layoutIndex = 0;
            }
            else
            {
                _layoutIndex++;
            }
            DoLayout();
        }

        public void ResetLayout()
        {
            GetLayoutEngine().ResetPrimaryArea();
            DoLayout();
        }

        public void FocusLastFocusedWindow()
        {
            if (_lastFocused != null)
            {
                _lastFocused.Focus();
            } else
            {
                FocusPrimaryWindow();
            }
        }

        public void FocusNextWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            var didFocus = false;
            for (var i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                if (window.IsFocused)
                {
                    if (i + 1 == windows.Count)
                    {
                        windows[0].Focus();
                    }
                    else
                    {
                        windows[i + 1].Focus();
                    }
                    didFocus = true;
                    break;
                }
            }

            if (!didFocus && windows.Count > 0)
            {
                if (_lastFocused != null)
                {
                    _lastFocused.Focus();
                } else
                {
                    windows[0].Focus();
                }
            }
        }

        public void FocusPreviousWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            var didFocus = false;
            for (var i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                if (window.IsFocused)
                {
                    if (i == 0)
                    {
                        windows[windows.Count - 1].Focus();
                    }
                    else
                    {
                        windows[i - 1].Focus();
                    }
                    didFocus = true;
                    break;
                }
            }

            if (!didFocus && windows.Count > 0)
            {
                if (_lastFocused != null)
                {
                    _lastFocused.Focus();
                } else
                {
                    windows[0].Focus();
                }
            }
        }

        public void FocusPrimaryWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            if (windows.Count > 0)
            {
                windows[0].Focus();
            }
        }

        public void SwapFocusAndPrimaryWindow()
        {
            var windows = this.Windows.Where(w => w.CanLayout).ToList();
            if (windows.Count > 1)
            {
                var primary = windows[0];
                var focus = windows.FirstOrDefault(w => w.IsFocused);

                if (focus != null)
                {
                    SwapWindows(primary, focus);
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

        public void ShrinkPrimaryArea()
        {
            GetLayoutEngine().ShrinkPrimaryArea();
            DoLayout();
        }
        public void ExpandPrimaryArea()
        {
            GetLayoutEngine().ExpandPrimaryArea();
            DoLayout();
        }

        public void IncrementNumberOfPrimaryWindows()
        {
            GetLayoutEngine().IncrementNumInPrimary();
            DoLayout();
        }

        public void DecrementNumberOfPrimaryWindows()
        {
            GetLayoutEngine().DecrementNumInPrimary();
            DoLayout();
        }

        public void ToggleFocusedWindowTiling()
        {
            var window = FocusedWindow;

            if (window != null)
            {
                if (_floating.ContainsKey(window))
                    _floating.Remove(window);
                else
                    _floating[window] = true;
                DoLayout();
            }
        }

        public void SwapWindowToPoint(IWindow window, int x, int y)
        {
            if (_windows.Contains(window))
            {
                var index = GetLayoutSlotIndexForPoint(x, y);
                var destWindow = _windows.Count > index ? _windows[index] : null;

                if (destWindow != null && window != destWindow)
                {
                    SwapWindows(window, destWindow);
                }
            }
        }

        public bool IsPointInside(int x, int y)
        {
            var monitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(this);

            if (monitor != null)
            {
                return monitor.X <= x && x <= (monitor.X + monitor.Width) && monitor.Y <= y && y <= (monitor.Y + monitor.Height);
            } else
            {
                return false;
            }
        }

        private int GetLayoutSlotIndexForPoint(int x, int y)
        {
            var locations = CalcLayout();
            if (locations == null)
                return -1;
            var monitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(this);
            if (monitor == null)
                return -1;

            var adjustedLocations = locations.Select(loc => new WindowLocation(loc.X + monitor.X, loc.Y + monitor.Y,
                                loc.Width, loc.Height, loc.State)).ToList();

            var firstFit = adjustedLocations.FindIndex(l => l.IsPointInside(x, y));
            return firstFit;
        }

        private IEnumerable<IWindowLocation> CalcLayout()
        {
            var windows = this.Windows.Where(w => w.CanLayout && !_floating.ContainsKey(w)).ToList();
            var monitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(this);
            if (monitor != null)
            {
                return GetLayoutEngine().CalcLayout(windows, monitor.Width, monitor.Height);
            }
            return null;
        }

        public void DoLayout()
        {
            var windows = this.Windows.Where(w => w.CanLayout && !_floating.ContainsKey(w)).ToList();

            if (_context.Enabled)
            {
                var monitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(this);
                if (monitor != null)
                {
                    windows.ForEach(w => w.ShowInCurrentState());

                    var locations = GetLayoutEngine().CalcLayout(windows, monitor.Width, monitor.Height)
                        .ToArray();

                    using (var handle = _context.Windows.DeferWindowsPos(windows.Count))
                    {
                        for (var i = 0; i < locations.Length; i++)
                        {
                            var window = windows[i];
                            var loc = locations[i];

                            var adjustedLoc = new WindowLocation(loc.X + monitor.X, loc.Y + monitor.Y, 
                                loc.Width, loc.Height, loc.State);

                            if (!window.IsMouseMoving)
                            {
                                handle.DeferWindowPos(window, adjustedLoc);
                            }
                        }
                    }
                }
                else
                {
                    windows.ForEach(w => w.Hide());
                }
            }
            else
            {
                windows.ForEach(w => w.ShowInCurrentState());
            }
        }

        public override string ToString()
        {
            return Name;
        }

        private void SwapWindows(IWindow left, IWindow right)
        {
            Logger.Trace("SwapWindows[{0},{1}]", left, right);
            var leftIdx = _windows.FindIndex(w => w == left);
            var rightIdx = _windows.FindIndex(w => w == right);

            _windows[leftIdx] = right;
            _windows[rightIdx] = left;

            DoLayout();
        }

        private ILayoutEngine GetLayoutEngine()
        {
            return _layoutEngines[_layoutIndex];
        }
    }
}
