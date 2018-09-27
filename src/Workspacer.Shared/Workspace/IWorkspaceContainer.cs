using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IWorkspaceContainer
    {
        void AssignWorkspaceToMonitor(IWorkspace workspace, IMonitor monitor);

        IWorkspace GetNextWorkspace(IWorkspace currentWorkspace);
        IWorkspace GetPreviousWorkspace(IWorkspace currentWorkspace);
        IWorkspace GetWorkspaceAtIndex(IWorkspace currentWorkspace, int index);
        IWorkspace GetWorkspaceByName(IWorkspace currentWorkspace, string name);
        IWorkspace GetWorkspaceForMonitor(IMonitor monitor);
        IMonitor GetMonitorForWorkspace(IWorkspace workspace);
        IEnumerable<IWorkspace> GetWorkspaces(IWorkspace currentWorkspace);
        IEnumerable<IWorkspace> GetWorkspaces(IMonitor currentMonitor);
        IEnumerable<IWorkspace> GetAllWorkspaces();
    }
}
