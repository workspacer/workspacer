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
        private Timer _timer;
        private int _interval;
        private string _format;

        public TimeWidget(int interval, string format)
        {
            _interval = interval;
            _format = format;
        }

        public TimeWidget() : this(200, "hh:mm:ss tt") { }

        public string GetText()
        {
            return DateTime.Now.ToString(_format);
        }

        public void Initialize(IBarWidgetContext context)
        {
            _context = context;
            _timer = new Timer(_interval);
            _timer.Elapsed += (s, e) => context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
