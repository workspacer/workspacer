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
        public IWorkspace FocusedWorkspace => _focusedWorkspace;

        private List<IWorkspace> _workspaces;
        private IWorkspace _focusedWorkspace;

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
                _focusedWorkspace = _workspaces[index];

                var workspaces = _workspaces.Where(w => w != _focusedWorkspace);
                foreach (var w in workspaces)
                {
                    w.Hide();
                }
                _focusedWorkspace.Show();
            }
        }

        public void MoveFocusedWindowToWorkspace(int index)
        {
            if (index < _workspaces.Count && index >= 0)
            {
                var window = _focusedWorkspace?.FocusedWindow;
                var targetWorkspace = _workspaces[index];

                _focusedWorkspace?.WindowDestroyed(window);
                targetWorkspace.WindowCreated(window);
            }
        }

        public void AddWindow(IWindow window)
        {
            FocusedWorkspace?.WindowCreated(window);
        }

        public void RemoveWindow(IWindow window)
        {
            var workspace = _workspaces.FirstOrDefault(w => w.Windows.Contains(window));
            workspace?.WindowDestroyed(window);
        }

        public void UpdateWindow(IWindow window)
        {
            var workspace = _workspaces.FirstOrDefault(w => w.Windows.Contains(window));
            workspace?.WindowUpdated(window);
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
