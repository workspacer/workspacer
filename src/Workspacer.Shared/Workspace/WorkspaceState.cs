using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class WorkspaceState
    {
        public Dictionary<int, int> WindowsToWorkspaces { get; set; }
        public Dictionary<int, int> MonitorsToWorkspaces { get; set; }
        public int FocusedMonitor { get; set; }
        public int FocusedWindow { get; set; }
    }
}
