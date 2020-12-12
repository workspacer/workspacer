using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace workspacer.Bar.Widgets
{
    public class TimeWidget : BarWidgetBase
    {
        
        private Timer _timer;
        private int _interval;
        private string _format;

        public TimeWidget(int interval, string format)
        {
            _interval = interval;
            _format = format;
        }

        public TimeWidget() : this(200, "hh:mm:ss tt") { }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts(Part(DateTime.Now.ToString(_format), fontname: FontName));
        }

        public override void Initialize()
        {
            _timer = new Timer(_interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
