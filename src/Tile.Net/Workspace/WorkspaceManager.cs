using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tile.Net.ConfigLoader;

namespace Tile.Net
{
    public class WorkspaceManager : IManager, IWorkspaceManager
    {
        public IEnumerable<IWorkspace> Workspaces => _workspaces;
        public IWorkspace FocusedWorkspace => _workspaces[_focusedWorkspace];
        public Func<IWindow, IWorkspace> WorkspaceSelectorFunc { get; set; }
        public Func<IWindow, bool> WindowFilterFunc { get; set; }

        private List<IWorkspace> _workspaces;
        private Dictionary<IWindow, IWorkspace> _windowsToWorkspaces;

        private int _focusedWorkspace;

        private WorkspaceManager()
        {
            _workspaces = new List<IWorkspace>();
            _windowsToWorkspaces = new Dictionary<IWindow, IWorkspace>();
            _focusedWorkspace = 0;
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
                _focusedWorkspace = index;

                var workspaces = _workspaces.Where(w => w != FocusedWorkspace);
                foreach (var w in workspaces)
                {
                    w.Hide();
                }
                FocusedWorkspace.Show();
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

        private void AddWindowToWorkspace(IWindow window, IWorkspace workspace)
        {
            workspace.AddWindow(window);
            _windowsToWorkspaces[window] = workspace;
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
            return new WorkspaceState()
            {
                WindowsToWorkspaces = dict,
                FocusedWorkspace = _focusedWorkspace,
                FocusedWindow = focusedWindow
            };
        }

        public void InitializeWithState(WorkspaceState state, IEnumerable<IWindow> windows)
        {
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
            foreach (var w in windows)
            {
                AddWindow(w, false);
            }
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
