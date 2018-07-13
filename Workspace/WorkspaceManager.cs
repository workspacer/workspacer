using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class WorkspaceManager
    {
        public IEnumerable<IWorkspace> Workspaces => _workspaces;
        public IWorkspace FocusedWorkspace => _workspaces[_focusedWorkspace];
        public Func<IWindow, IWorkspace> WorkspaceSelectorFunc { get; set; }

        private List<IWorkspace> _workspaces;
        private int _focusedWorkspace;

        private WorkspaceManager()
        {
            _workspaces = new List<IWorkspace>();
        }

        public void AddWorkspace(IWorkspace workspace)
        {
            _workspaces.Add(workspace);
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
            if (WorkspaceSelectorFunc == null)
            {
                FocusedWorkspace.AddWindow(window);
            }
            else
            {
                var workspace = WorkspaceSelectorFunc(window);
                workspace.AddWindow(window);

                SwitchToWorkspace(_workspaces.IndexOf(workspace));
            }
        }

        public void RemoveWindow(IWindow window)
        {
            var workspace = _workspaces.FirstOrDefault(w => w.Windows.Contains(window));
            workspace?.RemoveWindow(window);
        }

        public void UpdateWindow(IWindow window)
        {
            var workspace = _workspaces.FirstOrDefault(w => w.Windows.Contains(window));
            workspace?.UpdateWindow(window);
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
            return _workspaces.FirstOrDefault(w => w.Windows.Contains(window));
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

        private static WorkspaceManager _instance;

        public static WorkspaceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance();
                }
                return _instance;
            }
        }

        private static WorkspaceManager CreateInstance()
        {
            return new WorkspaceManager();
        }
    }
}
