using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class WorkspaceManager : IManager
    {
        public IEnumerable<IWorkspace> Workspaces => _workspaces;
        public IWorkspace FocusedWorkspace => _workspaces[_focusedWorkspace];
        public Func<IWindow, IWorkspace> WorkspaceSelectorFunc { get; set; }

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
                }
            }
        }

        public void AddWindow(IWindow window)
        {
            AddWindow(window, true);
        }

        public void AddWindow(IWindow window, bool switchToWorkspace)
        {
            if (!_windowsToWorkspaces.ContainsKey(window))
            {
                if (WorkspaceSelectorFunc == null)
                {
                    FocusedWorkspace.AddWindow(window);
                    _windowsToWorkspaces[window] = FocusedWorkspace;
                }
                else
                {
                    var workspace = WorkspaceSelectorFunc(window);

                    if (workspace != null)
                    {
                        workspace.AddWindow(window);
                        _windowsToWorkspaces[window] = workspace;

                        if (switchToWorkspace)
                        {
                            SwitchToWorkspace(_workspaces.IndexOf(workspace));
                        }
                    }
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
                _windowsToWorkspaces[window].UpdateWindow(window);
            }
        }

        public void PreExitCleanup()
        {
            foreach (var ws in _workspaces)
            {
                foreach (var w in ws.Windows)
                {
                    w.ShowInCurrentState();
                }
            }
        }

        public IWorkspace GetWorkspaceForWindow(IWindow window)
        {
            return _windowsToWorkspaces[window];
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
