using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tile.Net.ConfigLoader;

namespace Tile.Net
{
    public class WorkspaceManager : IManager, IWorkspaceManager
    {
        public IEnumerable<IMonitor> Monitors => _monitors;
        public IEnumerable<IWorkspace> Workspaces => _workspaces;
        public Func<IWindow, IWorkspace> WorkspaceSelectorFunc { get; set; }
        public Func<IWindow, bool> WindowFilterFunc { get; set; }

        private int _focusedMonitor;
        public IMonitor FocusedMonitor => _monitors[_focusedMonitor];
        public IWorkspace FocusedWorkspace => _workspaces.First(w => w.Monitor == FocusedMonitor);

        private List<IMonitor> _monitors;
        private List<IWorkspace> _workspaces;
        private Dictionary<IWindow, IWorkspace> _windowsToWorkspaces;

        public event WorkspaceUpdatedDelegate WorkspaceUpdated;
        public event FocusedMonitorUpdatedDelegate FocusedMonitorUpdated;

        private WorkspaceManager()
        {
            _monitors = new List<IMonitor>();
            _workspaces = new List<IWorkspace>();
            _windowsToWorkspaces = new Dictionary<IWindow, IWorkspace>();
            _focusedMonitor = 0;
        }
        public static WorkspaceManager Instance { get; } = new WorkspaceManager();

        public void AddWorkspace(string name, ILayoutEngine[] layouts)
        {
            if (_workspaces.All(w => w.Name != name))
            {
                _workspaces.Add(new Workspace(name, layouts));
            }
        }

        public void SwitchToWorkspace(int index)
        {
            if (index < _workspaces.Count && index >= 0)
            {
                var oldWorkspace = FocusedWorkspace;
                var newWorkspace = _workspaces[index];
                var srcMonitor = newWorkspace.Monitor;
                var destMonitor = FocusedMonitor;

                if (oldWorkspace != newWorkspace)
                {
                    AssignWorkspaceMonitor(destMonitor, newWorkspace);
                    AssignWorkspaceMonitor(srcMonitor, oldWorkspace);
                    WorkspaceUpdated?.Invoke();

                    oldWorkspace.DoLayout();
                    newWorkspace.DoLayout();

                    var window = newWorkspace.Windows.Where(w => w.CanLayout).FirstOrDefault();
                    if (window != null)
                    {
                        window.IsFocused = true;
                    }
                }
            }
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
            if (index < _workspaces.Count && index >= 0)
            {
                var window = FocusedWorkspace.FocusedWindow;
                var targetWorkspace = _workspaces[index];

                if (window != null)
                {
                    FocusedWorkspace.RemoveWindow(window);
                    targetWorkspace.AddWindow(window);
                    _windowsToWorkspaces[window] = targetWorkspace;
                    window.IsFocused = true;
                }
            }
        }

        public void MoveFocusedWindowToMonitor(int index)
        {
            if (index < _monitors.Count && index >= 0)
            {
                var destMonitor = _monitors[index];
                var destWorkspace = _workspaces.First(w => w.Monitor == destMonitor);
                var window = FocusedWorkspace.FocusedWindow;

                if (window != null)
                {
                    FocusedWorkspace.RemoveWindow(window);
                    destWorkspace.AddWindow(window);
                    _windowsToWorkspaces[window] = destWorkspace;
                    window.IsFocused = true;
                }
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
                            SwitchToWorkspace(_workspaces.IndexOf(workspace));
                        }
                    }
                }
            }
        }

        private void AssignWorkspaceMonitor(IMonitor monitor, IWorkspace workspace)
        {
            workspace.Monitor = monitor;
        }

        private void ClearWorkspaceMonitor(IWorkspace workspace)
        {
            workspace.Monitor = null;
        }

        private void AddWindowToWorkspace(IWindow window, IWorkspace workspace)
        {
            workspace.AddWindow(window);
            _windowsToWorkspaces[window] = workspace;

            if (window.IsFocused)
            {
                if (workspace.Monitor != null)
                {
                    _focusedMonitor = _monitors.IndexOf(workspace.Monitor);
                }
            }
        }

        public void RemoveWindow(IWindow window)
        {
            if (_windowsToWorkspaces.ContainsKey(window))
            {
                _windowsToWorkspaces[window].RemoveWindow(window);
                _windowsToWorkspaces.Remove(window);
            }
        }

        public void UpdateWindow(IWindow window)
        {
            if (_windowsToWorkspaces.ContainsKey(window))
            {
                if (window.IsFocused)
                {
                    var workspace = _windowsToWorkspaces[window];
                    if (workspace.Monitor != null)
                    {
                        _focusedMonitor = _monitors.IndexOf(workspace.Monitor);
                    }
                }

                _windowsToWorkspaces[window].UpdateWindow(window);
            }
        }

        public List<IntPtr> GetActiveHandles()
        {
            var list = new List<IntPtr>();
            foreach (var ws in _workspaces)
            {
                foreach (var w in ws.Windows)
                {
                    list.Add(w.Handle);
                }
            }
            return list;
        }

        public IWorkspace GetWorkspaceForWindow(IWindow window)
        {
            return _windowsToWorkspaces[window];
        }

        public WorkspaceState GetState()
        {
            var windowsToWorkspaces = new Dictionary<int, int>();
            var monitorsToWorkspaces = new Dictionary<int, int>();

            int focusedWindow = 0;
            foreach (var kv in _windowsToWorkspaces)
            {
                var index = _workspaces.IndexOf(kv.Value);
                windowsToWorkspaces[(int) kv.Key.Handle] = index;

                if (kv.Key.IsFocused)
                {
                    focusedWindow = (int)kv.Key.Handle;
                }
            }

            for (var i = 0; i < _monitors.Count; i++)
            {
                for (var j = 0; j < _workspaces.Count; j++)
                {
                    var monitor = _monitors[i];
                    var workspace = _workspaces[j];

                    if (workspace.Monitor == monitor)
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

        private void InitializeMonitors(bool assignWorkspaces = true)
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

            // check to make sure there are enough workspaces for the monitors
            if (_monitors.Count > _workspaces.Count)
            {
                throw new Exception("you must specify at least enough workspaces to cover all monitors");
            }

            for (var i = 0; i < _monitors.Count; i++)
            {
                var m = _monitors[i];
                var w = _workspaces[i];
                AssignWorkspaceMonitor(m, w);
            }
        }

        public void InitializeWithState(WorkspaceState state, IEnumerable<IWindow> windows)
        {
            InitializeMonitors(false);

            var wtw = state.WindowsToWorkspaces;

            foreach (var w in windows)
            {
                var shouldTrack = WindowFilterFunc?.Invoke(w) ?? true;
                if (!shouldTrack)
                    continue;

                var handle = (int) w.Handle;
                if (wtw.ContainsKey(handle) && wtw[handle] < _workspaces.Count)
                {
                    var workspace = _workspaces[wtw[handle]];
                    AddWindowToWorkspace(w, workspace);
                }
                else
                {
                    AddWindowToWorkspace(w, FocusedWorkspace);
                }

                if (state.FocusedWindow == handle)
                {
                    w.IsFocused = true;
                }
            }

            var mtw = state.MonitorsToWorkspaces;
            for (var i = 0; i < _monitors.Count; i++)
            {
                var workspaceIdx = mtw[i];
                var workspace = _workspaces[workspaceIdx];
                var monitor = _monitors[i];
                workspace.Monitor = monitor;
            }
        }

        public void Initialize(IEnumerable<IWindow> windows)
        {
            InitializeMonitors();

            foreach (var w in windows)
            {
                var shouldTrack = WindowFilterFunc?.Invoke(w) ?? true;
                if (!shouldTrack)
                    continue;

                var location = w.Location;
                var screen = Screen.FromRectangle(new Rectangle(location.X, location.Y, location.Width, location.Height));
                var monitor = _monitors.First(m => m.Name == screen.DeviceName);
                var workspace = _workspaces.First(wk => wk.Monitor == monitor);

                AddWindowToWorkspace(w, workspace);

                AddWindow(w, false);
            }
        }

        public IMonitor GetMonitorForWorkspace(IWorkspace workspace)
        {
            return workspace.Monitor;
        }

        public IWorkspace GetWorkspaceForMonitor(IMonitor monitor)
        {
            return _workspaces.FirstOrDefault(w => w.Monitor == monitor);
        }

        public IWorkspace this[int index] => _workspaces[index];

        public IWorkspace this[string name]
        {
            get
            {
                var workspace = _workspaces.FirstOrDefault(w => w.Name == name);
                if (workspace == null)
                {
                    throw new IndexOutOfRangeException();
                }
                return workspace;
            }
        }
    }
}
