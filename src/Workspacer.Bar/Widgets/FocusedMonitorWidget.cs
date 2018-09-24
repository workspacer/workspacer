using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.Bar.Widgets
{
    public class FocusedMonitorWidget : BarWidgetBase
    {
        public override IBarWidgetPart[] GetParts()
        {
            if (Context.Workspaces.FocusedMonitor == Context.Monitor)
            {
                return Parts("**********");
            } else
            {
                return Parts("");
            }
        }

        public override void Initialize()
        {
            Context.Workspaces.FocusedMonitorUpdated += () => Context.MarkDirty();
        }
    }
}
