using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Workspacer.Bar.Widgets
{
    public class ActiveLayoutWidget : BarWidgetBase
    {
        private Timer _timer;

        public ActiveLayoutWidget()
        {
        }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts("[" + Context.Monitor.Workspace.LayoutName + "]");
        }

        public override void Initialize()
        {
            _timer = new Timer(200);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
