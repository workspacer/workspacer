using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class WorkspaceContainer : IWorkspaceContainer
    {
        private IConfigContext _context;
        private Func<ILayoutEngine[]> _defaultLayouts;

        private List<IWorkspace> _workspaces;
        private Dictionary<IWorkspace, int> _workspaceMap;

        private Dictionary<IMonitor, IWorkspace> _mtw;
    
        public WorkspaceContainer(IConfigContext context, Func<ILayoutEngine[]> defaultLayouts)
        {
            _context = context;
            _defaultLayouts = defaultLayouts;

            _workspaces = new List<IWorkspace>();
            _workspaceMap = new Dictionary<IWorkspace, int>();

            _mtw = new Dictionary<IMonitor, IWorkspace>();
        }

        public void CreateWorkspaces(params string[] names)
        {
            foreach (var name in names)
            {
                CreateWorkspace(name, _defaultLayouts());
            }
        }

        public void CreateWorkspace(string name, params ILayoutEngine[] layouts)
        {
            layouts = layouts.Length > 0 ? layouts : _defaultLayouts();
            var workspace = new Workspace(_context, name, layouts);
            _workspaces.Add(workspace);
            _workspaceMap[workspace] = _workspaces.Count - 1;
            _context.Workspaces.ForceWorkspaceUpdate();
        }

        public void RemoveWorkspace(IWorkspace workspace)
        {
            var index = _workspaces.IndexOf(workspace);
            var dest = GetPreviousWorkspace(workspace);

            var monitor = GetCurrentMonitorForWorkspace(workspace);
            if (monitor != null)
            {
                var oldDestMonitor = GetCurrentMonitorForWorkspace(dest);
                if (oldDestMonitor != null)
                {
                    var newWorkspace = GetWorkspaces(oldDestMonitor).First(w => GetCurrentMonitorForWorkspace(w) == null);
                    AssignWorkspaceToMonitor(newWorkspace, oldDestMonitor);
                }
                AssignWorkspaceToMonitor(dest, monitor);
            }
            _context.Workspaces.MoveAllWindows(workspace, dest);

            for (var i = index + 1; i < _workspaces.Count; i++)
            {
                var w = _workspaces[i];
                _workspaceMap[w]--;
            }
            _workspaces.RemoveAt(index);

            _context.Workspaces.ForceWorkspaceUpdate();
        }

        public void AssignWorkspaceToMonitor(IWorkspace workspace, IMonitor monitor)
        {
            if (monitor != null)
            {
                _mtw[monitor] = workspace;
            }
        }

        public IWorkspace GetNextWorkspace(IWorkspace currentWorkspace)
        {
            VerifyExists(currentWorkspace);
            var index = _workspaceMap[currentWorkspace];
            if (index >= _workspaces.Count - 1)
                index = 0;
            else
                index = index + 1;

            return _workspaces[index];
        }

        public IWorkspace GetPreviousWorkspace(IWorkspace currentWorkspace)
        {
            VerifyExists(currentWorkspace);
            var index = _workspaceMap[currentWorkspace];
            if (index == 0)
                index = _workspaces.Count - 1;
            else
                index = index - 1;

            return _workspaces[index];
        }

        public IWorkspace GetWorkspaceAtIndex(IWorkspace currentWorkspace, int index)
        {
            VerifyExists(currentWorkspace);
            if (index >= _workspaces.Count)
                return null;

            return _workspaces[index];
        }

        public IMonitor GetCurrentMonitorForWorkspace(IWorkspace workspace)
        {
            return _mtw.Keys.FirstOrDefault(m => _mtw[m] == workspace);
        }

        public IMonitor GetDesiredMonitorForWorkspace(IWorkspace workspace)
        {
            return null;
        }

        public IWorkspace GetWorkspaceForMonitor(IMonitor monitor)
        {
            return _mtw[monitor];
        }

        public IEnumerable<IWorkspace> GetWorkspaces(IWorkspace currentWorkspace)
        {
            VerifyExists(currentWorkspace);
            return _workspaces;
        }

        public IEnumerable<IWorkspace> GetWorkspaces(IMonitor currentMonitor)
        {
            return _workspaces;
        }

        public IEnumerable<IWorkspace> GetAllWorkspaces()
        {
            return _workspaces;
        }

        public IWorkspace this[string name]
        {
            get
            {
                return _workspaces.FirstOrDefault(w => w.Name == name);
            }
        }

        private void VerifyExists(IWorkspace workspace)
        {
            if (!_workspaceMap.ContainsKey(workspace))
                throw new Exception("attempted to access container using a workspace that isn't contained in it");
        }
    }
}
