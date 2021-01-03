using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace workspacer.Bar.Widgets
{
    public class KeyModeWidget : BarWidgetBase
    {
      
        private Timer _timer;

        public KeyModeWidget() { }

        public override IBarWidgetPart[] GetParts()
        {
            var currentMode = Context.Keybinds.GetModeName();
            return Parts(Part(currentMode, fontname: FontName));
        }

        public override void Initialize()
        {
            _timer = new Timer(200);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
