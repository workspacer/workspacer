using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar
{
    public class BarWidgetContext : IBarWidgetContext
    {
        public IMonitor Monitor { get; private set; }
        public IWorkspaceManager Workspaces { get; private set; }

        private BarSection _section;

        public BarWidgetContext(BarSection section, IMonitor monitor, IWorkspaceManager workspaces)
        {
            _section = section;
            Monitor = monitor;
            Workspaces = workspaces;
        }

        public void MarkDirty()
        {
            _section.MarkDirty();
        }
    }
}
