using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public class WorkspaceState
    {
        public List<List<int>> WorkspacesToWindows { get; set; }
        public List<int> MonitorsToWorkspaces { get; set; }
        public int FocusedMonitor { get; set; }
        public int FocusedWindow { get; set; }
    }
}
