using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Tile.Net.Bar.Widgets
{
    public class TimeWidget : IBarWidget
    {
        private IBarWidgetContext _context;
        private string _format;
        private int _refresh;
        private Timer _timer;

        public TimeWidget(string format = "h:mm:ss tt", int refresh = 200)
        {
            _format = format;
            _refresh = refresh;
        }

        public string GetText()
        {
            return DateTime.Now.ToString(_format);
        }

        public void Initialize(IBarWidgetContext context)
        {
            _context = context;

            _timer = new Timer(_refresh);
            _timer.Elapsed += (s, e) => _context.Redraw();
            _timer.Enabled = true;
        }
    }
}
