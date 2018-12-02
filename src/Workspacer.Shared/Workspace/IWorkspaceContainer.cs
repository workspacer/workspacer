using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface IWorkspaceContainer
    {
        void AssignWorkspaceToMonitor(IWorkspace workspace, IMonitor monitor);

        void CreateWorkspaces(params string[] names);
        void CreateWorkspace(string name, params ILayoutEngine[] layouts);
        void RemoveWorkspace(IWorkspace workspace);

        IWorkspace GetNextWorkspace(IWorkspace currentWorkspace);
        IWorkspace GetPreviousWorkspace(IWorkspace currentWorkspace);

        IWorkspace GetWorkspaceAtIndex(IWorkspace currentWorkspace, int index);

        IWorkspace GetWorkspaceForMonitor(IMonitor monitor);
        IMonitor GetCurrentMonitorForWorkspace(IWorkspace workspace);
        IMonitor GetDesiredMonitorForWorkspace(IWorkspace workspace);

        IEnumerable<IWorkspace> GetWorkspaces(IWorkspace currentWorkspace);
        IEnumerable<IWorkspace> GetWorkspaces(IMonitor currentMonitor);
        IEnumerable<IWorkspace> GetAllWorkspaces();

        IWorkspace this[string name] { get; }
    }
}
