using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar.Widgets
{
    public class FocusedMonitorWidget : BarWidgetBase
    {
       

        public override IBarWidgetPart[] GetParts()
        {
            if (Context.MonitorContainer.FocusedMonitor == Context.Monitor)
            {
                return Parts(Part("**********", fontname: FontName));
            } else
            {
                return Parts(Part("", fontname: FontName));
            }
        }

        public override void Initialize()
        {
            Context.Workspaces.FocusedMonitorUpdated += () => Context.MarkDirty();
        }
    }
}
