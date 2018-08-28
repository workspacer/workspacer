using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class WorkspaceState
    {
        public Dictionary<int, int> WindowsToWorkspaces { get; set; }
        public int FocusedWorkspace { get; set; }
        public int FocusedWindow { get; set; }
    }
}
