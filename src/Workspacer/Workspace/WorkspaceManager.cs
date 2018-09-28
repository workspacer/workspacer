using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Workspacer.ConfigLoader;

namespace Workspacer
{
    public class WorkspaceManager : IManager, IWorkspaceManager
    {
        public IEnumerable<IMonitor> Monitors => _monitors;
        public Func<IWindow, IWorkspace> WorkspaceSelectorFunc { get; set; }
        public Func<IWindow, bool> WindowFilterFunc { get; set; }

        private int _focusedMonitor;
        public IMonitor FocusedMonitor => _monitors[_focusedMonitor];
        public IWorkspace FocusedWorkspace => Container.GetWorkspaceForMonitor(FocusedMonitor);

        private List<IMonitor> _monitors;
        private Dictionary<IWindow, IWorkspace> _windowsToWorkspaces;

        public IWorkspaceContainer Container { get; set; }

        public event WorkspaceUpdatedDelegate WorkspaceUpdated;
        public event WindowAddedDelegate WindowAdded;
        public event WindowUpdatedDelegate WindowUpdated;
        public event WindowRemovedDelegate WindowRemoved;
        public event WindowMovedDelegate WindowMoved;
        public event FocusedMonitorUpdatedDelegate FocusedMonitorUpdated;

        public WorkspaceManager()
        {
            _monitors = new List<IMonitor>();
            _windowsToWorkspaces = new Dictionary<IWindow, IWorkspace>();
            _focusedMonitor = 0;
        }

        public void SwitchToWorkspace(int index)
        {
            var currentWorkspace = FocusedWorkspace;
            var targetWorkspace = Container.GetWorkspaceAtIndex(currentWorkspace, index);
            SwitchToWorkspace(targetWorkspace);
        }

        private void SwitchToWorkspace(IWorkspace targetWorkspace)
        {
            if (targetWorkspace != null)
            {
                var destMonitor = Container.GetDesiredMonitorForWorkspace(targetWorkspace) ?? FocusedMonitor;
                var currentWorkspace = Container.GetWorkspaceForMonitor(destMonitor);
                var sourceMonitor = Container.GetCurrentMonitorForWorkspace(targetWorkspace);

                Container.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
                Container.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

                currentWorkspace.DoLayout();
                targetWorkspace.DoLayout();

                WorkspaceUpdated?.Invoke();

                targetWorkspace.FocusPrimaryWindow();
            }
        }

        public void SwitchMonitorToWorkspace(int monitorIndex, int workspaceIndex)
        {
            if (monitorIndex >= _monitors.Count)
                return;

            var destMonitor = _monitors[monitorIndex];
            var currentWorkspace = Container.GetWorkspaceForMonitor(destMonitor);
            var targetWorkspace = Container.GetWorkspaceAtIndex(currentWorkspace, workspaceIndex);
            var sourceMonitor = Container.GetCurrentMonitorForWorkspace(targetWorkspace);

            Container.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
            Container.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

            currentWorkspace.DoLayout();
            targetWorkspace.DoLayout();

            WorkspaceUpdated?.Invoke();

            targetWorkspace.FocusPrimaryWindow();
        }

        public void SwitchToNextWorkspace()
        {
            var destMonitor = FocusedMonitor;
            var currentWorkspace = Container.GetWorkspaceForMonitor(destMonitor);
            var targetWorkspace = Container.GetNextWorkspace(currentWorkspace);
            var sourceMonitor = Container.GetCurrentMonitorForWorkspace(targetWorkspace);

            Container.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
            Container.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

            currentWorkspace.DoLayout();
            targetWorkspace.DoLayout();

            WorkspaceUpdated?.Invoke();

            targetWorkspace.FocusPrimaryWindow();
        }

        public void SwitchToPreviousWorkspace()
        {
            var destMonitor = FocusedMonitor;
            var currentWorkspace = Container.GetWorkspaceForMonitor(destMonitor);
            var targetWorkspace = Container.GetPreviousWorkspace(currentWorkspace);
            var sourceMonitor = Container.GetCurrentMonitorForWorkspace(targetWorkspace);

            Container.AssignWorkspaceToMonitor(currentWorkspace, sourceMonitor);
            Container.AssignWorkspaceToMonitor(targetWorkspace, destMonitor);

            currentWorkspace.DoLayout();
            targetWorkspace.DoLayout();

            WorkspaceUpdated?.Invoke();

            targetWorkspace.FocusPrimaryWindow();
        }

        public void SwitchFocusedMonitor(int index)
        {
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
            var window = FocusedWorkspace.FocusedWindow;
            var targetWorkspace = Container.GetWorkspaceAtIndex(FocusedWorkspace, index);

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
            if (index >= _monitors.Count)
                return;

            var window = FocusedWorkspace.FocusedWindow;
            var targetMonitor = _monitors[index];
            var targetWorkspace = Container.GetWorkspaceForMonitor(targetMonitor);

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

        public void AddWindow(IWindow window)
        {
            AddWindow(window, true);
        }

        public void AddWindow(IWindow window, bool switchToWorkspace)
        {
            var shouldTrack = WindowFilterFunc?.Invoke(window) ?? true;

            if (!shouldTrack)
                return;

            if (!_windowsToWorkspaces.ContainsKey(window))
            {
                if (WorkspaceSelectorFunc == null)
                {
                    AddWindowToWorkspace(window, FocusedWorkspace);
                }
                else
                {
                    var workspace = WorkspaceSelectorFunc(window);

                    if (workspace != null)
                    {
                        AddWindowToWorkspace(window, workspace);

                        if (switchToWorkspace)
                        {
                            SwitchToWorkspace(workspace);
                        }
                    }
                }
            }
        }

        private void AddWindowToWorkspace(IWindow window, IWorkspace workspace)
        {
            workspace.AddWindow(window);
            _windowsToWorkspaces[window] = workspace;

            if (window.IsFocused)
            {
                var monitor = Container.GetCurrentMonitorForWorkspace(workspace);
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
                var workspace = _windowsToWorkspaces[window];
                _windowsToWorkspaces[window].RemoveWindow(window);
                _windowsToWorkspaces.Remove(window);
                WindowRemoved?.Invoke(window, workspace);
            }
        }

        public void UpdateWindow(IWindow window)
        {
            if (_windowsToWorkspaces.ContainsKey(window))
            {
                var workspace = _windowsToWorkspaces[window];
                if (window.IsFocused)
                {
                var monitor = Container.GetCurrentMonitorForWorkspace(workspace);
                    if (monitor != null)
                    {
                        _focusedMonitor = _monitors.IndexOf(monitor);
                    }
                }

                _windowsToWorkspaces[window].UpdateWindow(window);
                WindowUpdated?.Invoke(window, workspace);
            }
        }

        public List<IntPtr> GetActiveHandles()
        {
            var list = new List<IntPtr>();
            foreach (var ws in Container.GetAllWorkspaces())
            {
                foreach (var w in ws.Windows.Where(w => w.CanLayout))
                {
                    list.Add(w.Handle);
                }
            }
            return list;
        }

        public WorkspaceState GetState()
        {
            var windowsToWorkspaces = new Dictionary<int, int>();
            var monitorsToWorkspaces = new Dictionary<int, int>();

            var allWorkspaces = Container.GetAllWorkspaces().ToList();

            int focusedWindow = 0;
            foreach (var kv in _windowsToWorkspaces)
            {
                var index = allWorkspaces.IndexOf(kv.Value);
                windowsToWorkspaces[(int) kv.Key.Handle] = index;

                if (kv.Key.IsFocused)
                {
                    focusedWindow = (int)kv.Key.Handle;
                }
            }

            for (var i = 0; i < _monitors.Count; i++)
            {
                for (var j = 0; j < allWorkspaces.Count; j++)
                {
                    var monitor = _monitors[i];
                    var workspace = allWorkspaces[j];

                    if (Container.GetCurrentMonitorForWorkspace(workspace) == monitor)
                    {
                        monitorsToWorkspaces[i] = j;
                    }
                }
            }

            var monitorIndex = _monitors.IndexOf(FocusedMonitor);

            return new WorkspaceState()
            {
                WindowsToWorkspaces = windowsToWorkspaces,
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

        public void InitializeWithState(WorkspaceState state, IEnumerable<IWindow> windows)
        {
            var wtw = state.WindowsToWorkspaces;
            var allWorkspaces = Container.GetAllWorkspaces().ToList();

            foreach (var w in windows)
            {
                var shouldTrack = WindowFilterFunc?.Invoke(w) ?? true;
                if (!shouldTrack)
                    continue;

                var handle = (int) w.Handle;
                if (wtw.ContainsKey(handle) && wtw[handle] < allWorkspaces.Count)
                {
                    var workspace = allWorkspaces[wtw[handle]];
                    AddWindowToWorkspace(w, workspace);
                }
                else
                {
                    var destWorkspace = WorkspaceSelectorFunc?.Invoke(w) ?? FocusedWorkspace;
                    AddWindowToWorkspace(w, destWorkspace);
                }

                if (state.FocusedWindow == handle)
                {
                    w.Focus();
                }
            }

            var mtw = state.MonitorsToWorkspaces;
            for (var i = 0; i < _monitors.Count; i++)
            {
                var workspaceIdx = mtw[i];
                var workspace = allWorkspaces[workspaceIdx];
                var monitor = _monitors[i];
                Container.AssignWorkspaceToMonitor(workspace, monitor);
            }
        }

        public void Initialize(IEnumerable<IWindow> windows)
        {
            var allWorkspaces = Container.GetAllWorkspaces().ToList();
            for (var i = 0; i < _monitors.Count; i++)
            {
                var m = _monitors[i];
                var w = allWorkspaces[i];
                Container.AssignWorkspaceToMonitor(w, m);
            }
            
            foreach (var w in windows)
            {
                var shouldTrack = WindowFilterFunc?.Invoke(w) ?? true;
                if (!shouldTrack)
                    continue;

                var destWorkspace = WorkspaceSelectorFunc?.Invoke(w);

                if (destWorkspace == null)
                {
                    var location = w.Location;
                    var screen = Screen.FromRectangle(new Rectangle(location.X, location.Y, location.Width, location.Height));
                    var monitor = _monitors.First(m => m.Name == screen.DeviceName);
                    destWorkspace = Container.GetWorkspaceForMonitor(monitor);
                }

                AddWindowToWorkspace(w, destWorkspace);

                AddWindow(w, false);
            }
        }
    }
}
