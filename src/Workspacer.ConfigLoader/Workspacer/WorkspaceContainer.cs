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

        private List<IWorkspace> _workspaces;
        private Dictionary<IWorkspace, int> _workspaceMap;

        private Dictionary<IMonitor, IWorkspace> _mtw;
 
        public WorkspaceContainer(IConfigContext context)
        {
            _context = context;

            _workspaces = new List<IWorkspace>();
            _workspaceMap = new Dictionary<IWorkspace, int>();

            _mtw = new Dictionary<IMonitor, IWorkspace>();
        }

        public void CreateWorkspace(string name, params ILayoutEngine[] layouts)
        {
            var workspace = new Workspace(_context, name, layouts);
            _workspaces.Add(workspace);
            _workspaceMap[workspace] = _workspaces.Count - 1;
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

        public IWorkspace GetWorkspaceByName(IWorkspace currentWorkspace, string name)
        {
            VerifyExists(currentWorkspace);
            return _workspaces.FirstOrDefault(w => w.Name == name);
        }

        public IMonitor GetMonitorForWorkspace(IWorkspace workspace)
        {
            return _mtw.Keys.FirstOrDefault(m => _mtw[m] == workspace);
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

        private void VerifyExists(IWorkspace workspace)
        {
            if (!_workspaceMap.ContainsKey(workspace))
                throw new Exception("attempted to access container using a workspace that isn't contained in it");
        }
    }
}
