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

        private BarForm _form;

        public BarWidgetContext(BarForm form, IMonitor monitor, IWorkspaceManager workspaces)
        {
            _form = form;
            Monitor = monitor;
            Workspaces = workspaces;
        }

        public void MarkDirty()
        {
            _form.MarkDirty();
        }
    }
}
