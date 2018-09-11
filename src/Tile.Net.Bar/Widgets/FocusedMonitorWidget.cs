using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar.Widgets
{
    public class FocusedMonitorWidget : IBarWidget
    {
        private IBarWidgetContext _context;

        public string GetText()
        {
            if (_context.Workspaces.FocusedMonitor == _context.Monitor)
            {
                return "**********";
            } else
            {
                return "";
            }
        }

        public void Initialize(IBarWidgetContext context)
        {
            _context = context;

            context.Workspaces.FocusedMonitorUpdated += () => context.Redraw();
        }
    }
}
