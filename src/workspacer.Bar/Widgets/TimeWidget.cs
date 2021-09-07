using System;
using System.Timers;

namespace workspacer.Bar.Widgets
{
    public class TimeWidget : BarWidgetBase
    {
        
        private Timer _timer;
        private int _interval;
        private string _format;
        private string _fontstyle = null;

        public TimeWidget(int interval, string format, string style = null)
        {
            _interval = interval;
            _format = format;
            _fontstyle = style;
        }

        public TimeWidget() : this(200, "dd.MM.yy") { }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts(Part(DateTime.Now.ToString(_format), fontname: FontName, fontstyle: _fontstyle));
        }

        public override void Initialize()
        {
            _timer = new Timer(_interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
