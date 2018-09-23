using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Tile.Net.Bar.Widgets
{
    public class ActiveLayoutWidget : IBarWidget
    {
        private IBarWidgetContext _context;
        private Timer _timer;

        public ActiveLayoutWidget()
        {
        }

        public string GetText()
        {
            return "[" + _context.Monitor.Workspace.LayoutName + "]";
        }

        public void Initialize(IBarWidgetContext context)
        {
            _context = context;
            _timer = new Timer(200);
            _timer.Elapsed += (s, e) => context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
