using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private int _focusedWorkspace;
        public IWorkspace FocusedWorkspace => _workspaces[_focusedWorkspace];

        private List<IMonitor> _monitors;
        private List<IWorkspace> _workspaces;
        private Dictionary<IWindow, IWorkspace> _windowsToWorkspaces;

        private Dictionary<IMonitor, IWorkspace> _monitorsToWorkspaces;
        private Dictionary<IWorkspace, IMonitor> _workspacesToMonitors;

        public event WorkspaceUpdatedDelegate WorkspaceUpdated;

        private WorkspaceManager()
        {
            _focusedWorkspace = 0;
            _monitors = new List<IMonitor>();
            _workspaces = new List<IWorkspace>();
            _windowsToWorkspaces = new Dictionary<IWindow, IWorkspace>();

            _monitorsToWorkspaces = new Dictionary<IMonitor, IWorkspace>();
            _workspacesToMonitors = new Dictionary<IWorkspace, IMonitor>();
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

                if (_workspacesToMonitors.ContainsKey(oldWorkspace))
                {
                    var monitor = _workspacesToMonitors[oldWorkspace];
                    var newWorkspace = _workspaces[index];

                    if (oldWorkspace != newWorkspace)
                    {
                        AssignWorkspaceToMonitor(monitor, newWorkspace);
                        _focusedWorkspace = index;

                        oldWorkspace.Hide();
                        newWorkspace.Show();
                        var window = newWorkspace.Windows.Where(w => w.CanLayout).FirstOrDefault();
                        if (window != null)
                        {
                            window.IsFocused = true;
                        }
                    } else
                    {
                        newWorkspace.Show();
                    }
                    WorkspaceUpdated?.Invoke();
                }
            };
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

        private void AssignWorkspaceToMonitor(IMonitor monitor, IWorkspace workspace)
        {
            var oldWorkspace = _monitorsToWorkspaces.ContainsKey(monitor) ? _monitorsToWorkspaces[monitor] : null;
            if (oldWorkspace != null)
            {
                _workspacesToMonitors.Remove(oldWorkspace);
            }
            _monitorsToWorkspaces[monitor] = workspace;
            _workspacesToMonitors[workspace] = monitor;

            workspace.SetMonitor(monitor);
        }

        private void AddWindowToWorkspace(IWindow window, IWorkspace workspace)
        {
            workspace.AddWindow(window);
            _windowsToWorkspaces[window] = workspace;

            if (window.IsFocused)
            {
                _focusedWorkspace = _workspaces.IndexOf(workspace);
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
                    _focusedWorkspace = _workspaces.IndexOf(_windowsToWorkspaces[window]);
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
            var dict = new Dictionary<int, int>();

            int focusedWindow = 0;
            foreach (var kv in _windowsToWorkspaces)
            {
                var index = _workspaces.IndexOf(kv.Value);
                dict[(int) kv.Key.Handle] = index;

                if (kv.Key.IsFocused)
                {
                    focusedWindow = (int)kv.Key.Handle;
                }
            }

            var workspaceIndex = _workspaces.IndexOf(FocusedWorkspace);
            return new WorkspaceState()
            {
                WindowsToWorkspaces = dict,
                FocusedWorkspace = workspaceIndex,
                FocusedWindow = focusedWindow
            };
        }

        private void InitializeMonitors()
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
                AssignWorkspaceToMonitor(m, w);
            }
        }

        public void InitializeWithState(WorkspaceState state, IEnumerable<IWindow> windows)
        {
            InitializeMonitors();
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
            _focusedWorkspace = state.FocusedWorkspace;
        }

        public void Initialize(IEnumerable<IWindow> windows)
        {
            InitializeMonitors();

            foreach (var w in windows)
            {
                AddWindow(w, false);
            }
        }

        public IMonitor GetMonitorForWorkspace(IWorkspace workspace)
        {
            return _workspacesToMonitors.ContainsKey(workspace) ? _workspacesToMonitors[workspace] : null;
        }

        public IWorkspace GetWorkspaceForMonitor(IMonitor monitor)
        {
            return _monitorsToWorkspaces.ContainsKey(monitor) ? _monitorsToWorkspaces[monitor] : null;
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
