using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using workspacer.ConfigLoader;

namespace workspacer
{
    public class WorkspaceManager : IManager, IWorkspaceManager
    {
        private static Logger Logger = Logger.Create();
        public IEnumerable<IMonitor> Monitors => _monitors;

        private IConfigContext _context;
        private int _focusedMonitor;
        public IMonitor FocusedMonitor => _monitors[_focusedMonitor];
        public IWorkspace FocusedWorkspace => _context.WorkspaceContainer.GetWorkspaceForMonitor(FocusedMonitor);

        private List<IMonitor> _monitors;
        private Dictionary<IWindow, IWorkspace> _windowsToWorkspaces;

        public event WorkspaceUpdatedDelegate WorkspaceUpdated;
        public event WindowAddedDelegate WindowAdded;
        public event WindowUpdatedDelegate WindowUpdated;
        public event WindowRemovedDelegate WindowRemoved;
        public event WindowMovedDelegate WindowMoved;
        public event FocusedMonitorUpdatedDelegate FocusedMonitorUpdated;

        public WorkspaceManager(IConfigContext context)
        {
            _context = context;
            _monitors = new List<IMonitor>();
            _windowsToWorkspaces = new Dictionary<IWindow, IWorkspace>();
            _focusedMonitor = 0;
        }

        public void SwitchToWindow(IWindow window)
        {
            Logger.Debug("SwitchToWindow({0})", window);

            if (_windowsToWorkspaces.ContainsKey(window))
            {
                var workspace = _windowsToWorkspaces[window];
                SwitchToWorkspace(workspace);
                window.Focus();
            }
        }

        public void SwitchToWorkspace(int index)
        {
            Logger.Debug("SwitchToWorkspace({0})", index);
            var currentWorkspace = FocusedWorkspace;
            var targetWorkspace = _context.WorkspaceContainer.GetWorkspaceAtIndex(currentWorkspace, index);
            SwitchToWorkspace(targetWorkspace);
        }

        public void SwitchToWorkspace(IWorkspace targetWorkspace)
        {
            Logger.Debug("SwitchToWorkspace({0})", targetWorkspace);
            if (targetWorkspace != null)
            {
                var destMonitor = _context.WorkspaceContainer.GetDesiredMonitorForWorkspace(targetWorkspace) ?? FocusedMonitor;
                var currentWorkspace = _context.WorkspaceContainer.GetWorkspaceForMonitor(destMonitor);
                var sourceMonitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(targetWorkspace);

                _context.WorkspaceContainer.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
                _context.WorkspaceContainer.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

                currentWorkspace.DoLayout();
                targetWorkspace.DoLayout();

                WorkspaceUpdated?.Invoke();

                targetWorkspace.FocusLastFocusedWindow();
            }
        }

        public void SwitchMonitorToWorkspace(int monitorIndex, int workspaceIndex)
        {
            Logger.Debug("SwitchMonitorToWorkspace(monitorIndex: {0}, workspaceIndex: {1})", monitorIndex, workspaceIndex);
            if (monitorIndex >= _monitors.Count)
                return;

            var destMonitor = _monitors[monitorIndex];
            var currentWorkspace = _context.WorkspaceContainer.GetWorkspaceForMonitor(destMonitor);
            var targetWorkspace = _context.WorkspaceContainer.GetWorkspaceAtIndex(currentWorkspace, workspaceIndex);
            var sourceMonitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(targetWorkspace);

            _context.WorkspaceContainer.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
            _context.WorkspaceContainer.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

            currentWorkspace.DoLayout();
            targetWorkspace.DoLayout();

            WorkspaceUpdated?.Invoke();

            targetWorkspace.FocusLastFocusedWindow();
        }

        public void SwitchToNextWorkspace()
        {
            Logger.Debug("SwitchToNextWorkspace");
            var destMonitor = FocusedMonitor;
            var currentWorkspace = _context.WorkspaceContainer.GetWorkspaceForMonitor(destMonitor);
            var targetWorkspace = _context.WorkspaceContainer.GetNextWorkspace(currentWorkspace);
            var sourceMonitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(targetWorkspace);

            _context.WorkspaceContainer.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
            _context.WorkspaceContainer.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

            currentWorkspace.DoLayout();
            targetWorkspace.DoLayout();

            WorkspaceUpdated?.Invoke();

            targetWorkspace.FocusLastFocusedWindow();
        }

        public void SwitchToPreviousWorkspace()
        {
            Logger.Debug("SwitchToPreviousWorkspace");
            var destMonitor = FocusedMonitor;
            var currentWorkspace = _context.WorkspaceContainer.GetWorkspaceForMonitor(destMonitor);
            var targetWorkspace = _context.WorkspaceContainer.GetPreviousWorkspace(currentWorkspace);
            var sourceMonitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(targetWorkspace);

            _context.WorkspaceContainer.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
            _context.WorkspaceContainer.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

            currentWorkspace.DoLayout();
            targetWorkspace.DoLayout();

            WorkspaceUpdated?.Invoke();

            targetWorkspace.FocusLastFocusedWindow();
        }

        public void SwitchFocusedMonitor(int index)
        {
            Logger.Debug("SwitchFocusedMonitor({0})", index);
            if (index < _monitors.Count && index >= 0)
            {
                if (_focusedMonitor != index)
                {
                    _focusedMonitor = index;
                    FocusedWorkspace.FocusLastFocusedWindow();

                    FocusedMonitorUpdated?.Invoke();
                }
            }
        }

        public void SwitchFocusedMonitorToMouseLocation()
        {
            Logger.Debug("SwitchFocusedMonitorToMouseLocation");
            var loc = Control.MousePosition;
            var screen = Screen.FromPoint(new Point(loc.X, loc.Y));
            var monitor = _monitors.First(m => m.Name == screen.DeviceName);

            for (var i = 0; i < _monitors.Count; i++)
            {
                if (_monitors[i] == monitor && _focusedMonitor != i)
                {
                    _focusedMonitor = i;
                    FocusedMonitorUpdated?.Invoke();
                    break;
                }
            }
        }

        public void MoveFocusedWindowToWorkspace(int index)
        {
            Logger.Debug("MoveFocusedWindowToWorkspace({0})", index);
            var window = FocusedWorkspace.FocusedWindow;
            var targetWorkspace = _context.WorkspaceContainer.GetWorkspaceAtIndex(FocusedWorkspace, index);

            if (window != null && targetWorkspace != null)
            {
                var windows = FocusedWorkspace.Windows.Where(w => w.CanLayout);
                // get next window
                var nextWindow = windows.SkipWhile(x => x != window).Skip(1).FirstOrDefault();
                if (nextWindow == null)
                {
                    // get previous window
                    nextWindow = windows.TakeWhile(x => x != window).LastOrDefault();
                }

                FocusedWorkspace.RemoveWindow(window);
                targetWorkspace.AddWindow(window);

                _windowsToWorkspaces[window] = targetWorkspace;
                WindowMoved?.Invoke(window, FocusedWorkspace, targetWorkspace);

                nextWindow?.Focus();
            }
        }

        public void MoveFocusedWindowToMonitor(int index)
        {
            Logger.Debug("MoveFocusedWindowToMonitor({0})", index);
            if (index >= _monitors.Count)
                return;

            var window = FocusedWorkspace.FocusedWindow;
            var targetMonitor = _monitors[index];
            var targetWorkspace = _context.WorkspaceContainer.GetWorkspaceForMonitor(targetMonitor);

            if (window != null && targetWorkspace != null)
            {
                var windows = FocusedWorkspace.Windows.Where(w => w.CanLayout);
                // get next window
                var nextWindow = windows.SkipWhile(x => x != window).Skip(1).FirstOrDefault();
                if (nextWindow == null)
                {
                    // get previous window
                    nextWindow = windows.TakeWhile(x => x != window).LastOrDefault();
                }

                FocusedWorkspace.RemoveWindow(window);
                targetWorkspace.AddWindow(window);

                _windowsToWorkspaces[window] = targetWorkspace;
                WindowMoved?.Invoke(window, FocusedWorkspace, targetWorkspace);

                nextWindow?.Focus();
            }
        }

        public void MoveAllWindows(IWorkspace source, IWorkspace dest)
        {
            var toMove = source.Windows.ToList();
            foreach (var window in toMove)
            {
                RemoveWindow(window);
            }
            foreach (var window in toMove)
            {
                AddWindowToWorkspace(window, dest);
            }
        }

        public void ForceWorkspaceUpdate()
        {
            WorkspaceUpdated?.Invoke();
        }

        public void AddWindow(IWindow window)
        {
            AddWindow(window, true);
        }

        public void AddWindow(IWindow window, bool switchToWorkspace)
        {
            Logger.Debug("AddWindow({0})", window);

            if (!_windowsToWorkspaces.ContainsKey(window))
            {
                var workspace = _context.WindowRouter.RouteWindow(window);

                if (workspace != null)
                {
                    AddWindowToWorkspace(window, workspace);

                    if (switchToWorkspace && window.CanLayout)
                    {
                        SwitchToWorkspace(workspace);
                    }
                }
            }
        }

        private void AddWindowToWorkspace(IWindow window, IWorkspace workspace)
        {
            Logger.Debug("AddWindowToWorkspace({0}, {1})", window, workspace);
            workspace.AddWindow(window);
            _windowsToWorkspaces[window] = workspace;

            if (window.IsFocused)
            {
                var monitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace);
                if (monitor != null)
                {
                    _focusedMonitor = _monitors.IndexOf(monitor);
                }
            }
            WindowAdded?.Invoke(window, workspace);
        }

        public void RemoveWindow(IWindow window)
        {
            if (_windowsToWorkspaces.ContainsKey(window))
            {
                Logger.Debug("RemoveWindow({0})", window);
                var workspace = _windowsToWorkspaces[window];
                _windowsToWorkspaces[window].RemoveWindow(window);
                _windowsToWorkspaces.Remove(window);
                WindowRemoved?.Invoke(window, workspace);
            }
        }

        public void UpdateWindow(IWindow window, WindowUpdateType type)
        {
            if (_windowsToWorkspaces.ContainsKey(window))
            {
                Logger.Trace("UpdateWindow({0})", window);
                var workspace = _windowsToWorkspaces[window];
                if (window.IsFocused)
                {
                    var monitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace);
                    if (monitor != null)
                    {
                        _focusedMonitor = _monitors.IndexOf(monitor);
                    } else
                    {
                        if (type == WindowUpdateType.Foreground)
                        {
                            // TODO: show flash for workspace (in bar?)
                            workspace.IsIndicating = true;
                            WorkspaceUpdated?.Invoke();
                        }
                    }
                }

                if (type == WindowUpdateType.Move)
                {
                    TrySwapWindowToMouse(window);
                }

                _windowsToWorkspaces[window].UpdateWindow(window, type);
                WindowUpdated?.Invoke(window, workspace);
            }
        }

        private void TrySwapWindowToMouse(IWindow window)
        {
            var point = Control.MousePosition;
            int x = point.X;
            int y = point.Y;

            var currentWorkspace = _windowsToWorkspaces[window];

            if (currentWorkspace.IsPointInside(x, y))
            {
                currentWorkspace.SwapWindowToPoint(window, x, y);
            } else
            {
                foreach (var workspace in _context.WorkspaceContainer.GetAllWorkspaces())
                {
                    var monitor = _context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace);
                    if (monitor != null && workspace.IsPointInside(x, y))
                    {
                        currentWorkspace.RemoveWindow(window, false);
                        workspace.AddWindow(window, false);
                        _windowsToWorkspaces[window] = workspace;

                        workspace.SwapWindowToPoint(window, x, y);
                        currentWorkspace.DoLayout();
                    }
                }
            }
        }

        public WorkspaceState GetState()
        {
            var workspacesToWindows = new List<List<int>>();
            var monitorsToWorkspaces = new Dictionary<int, int>();

            var allWorkspaces = _context.WorkspaceContainer.GetAllWorkspaces().ToList();

            int focusedWindow = 0;
            foreach (var workspace in allWorkspaces)
            {
                var windows = new List<int>();
                foreach (var window in workspace.Windows)
                {
                    windows.Add((int)window.Handle);

                    if (window.IsFocused)
                    {
                        focusedWindow = (int)window.Handle;
                    }
                }
                workspacesToWindows.Add(windows);
            }

            for (var i = 0; i < _monitors.Count; i++)
            {
                for (var j = 0; j < allWorkspaces.Count; j++)
                {
                    var monitor = _monitors[i];
                    var workspace = allWorkspaces[j];

                    if (_context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace) == monitor)
                    {
                        monitorsToWorkspaces[i] = j;
                    }
                }
            }

            var monitorIndex = _monitors.IndexOf(FocusedMonitor);

            return new WorkspaceState()
            {
                WorkspacesToWindows = workspacesToWindows,
                MonitorsToWorkspaces = monitorsToWorkspaces,
                FocusedMonitor = monitorIndex,
                FocusedWindow = focusedWindow
            };
        }

        public void InitializeMonitors()
        {
            var primary = Screen.PrimaryScreen;
            _monitors.Add(new Monitor(0, primary));

            int index = 1;
            foreach (var screen in Screen.AllScreens) {
                if (!screen.Primary)
                {
                    _monitors.Add(new Monitor(index, screen));
                    index++;
                }
            }

            
        }

        public void InitializeWithState(WorkspaceState state, IEnumerable<IWindow> allWindows)
        {
            var wtw = state.WorkspacesToWindows;
            var allWorkspaces = _context.WorkspaceContainer.GetAllWorkspaces().ToList();

            _focusedMonitor = state.FocusedMonitor;

            var mtw = state.MonitorsToWorkspaces;
            for (var i = 0; i < _monitors.Count; i++)
            {
                var workspaceIdx = mtw[i];
                var workspace = allWorkspaces[workspaceIdx];
                var monitor = _monitors[i];
                _context.WorkspaceContainer.AssignWorkspaceToMonitor(workspace, monitor);
            }

            for (var i = 0; i < wtw.Count; i++)
            {
                var workspaceWindows = wtw[i];
                for (var j = 0; j < wtw[i].Count; j++)
                {
                    var handle = workspaceWindows[j];
                    var window = allWindows.FirstOrDefault(w => (int)w.Handle == handle);

                    if (window == null)
                        continue;

                    var routedWorkspace = _context.WindowRouter.RouteWindow(window);
                    if (routedWorkspace == null)
                        continue;

                    if (i < allWorkspaces.Count)
                    {
                        // ignoring the routed workspace here, as the user probably put this window into
                        // the saved workspace on purpose
                        var workspace = allWorkspaces[i];
                        AddWindowToWorkspace(window, workspace);
                    }
                    else
                    {
                        AddWindowToWorkspace(window, routedWorkspace);
                    }

                    if (state.FocusedWindow == handle)
                    {
                        window.Focus();
                    }
                }
            }
        }

        public void Initialize(IEnumerable<IWindow> windows)
        {
            var allWorkspaces = _context.WorkspaceContainer.GetAllWorkspaces().ToList();
            for (var i = 0; i < _monitors.Count; i++)
            {
                var m = _monitors[i];
                var w = allWorkspaces[i];
                _context.WorkspaceContainer.AssignWorkspaceToMonitor(w, m);
            }
            
            foreach (var w in windows)
            {
                var location = w.Location;
                var screen = Screen.FromRectangle(new Rectangle(location.X, location.Y, location.Width, location.Height));
                var monitor = _monitors.First(m => m.Name == screen.DeviceName);
                var locationWorkspace = _context.WorkspaceContainer.GetWorkspaceForMonitor(monitor);
                var destWorkspace = _context.WindowRouter.RouteWindow(w, locationWorkspace);

                if (destWorkspace != null)
                {
                    AddWindowToWorkspace(w, destWorkspace);
                    AddWindow(w, false);
                }
            }
        }
    }
}
