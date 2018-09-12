using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar
{
    public interface IBarWidgetContext
    {
        IMonitor Monitor { get; }
        IWorkspaceManager Workspaces { get; }
        void MarkDirty();
    }
}
