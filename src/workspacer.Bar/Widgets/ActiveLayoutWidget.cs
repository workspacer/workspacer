using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace workspacer.Bar.Widgets
{
    public class ActiveLayoutWidget : BarWidgetBase
    {
      
        private Timer _timer;

        public ActiveLayoutWidget() { }

        public override IBarWidgetPart[] GetParts()
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            return Parts(Part(LeftPadding + currentWorkspace.LayoutName + RightPadding, partClicked: () =>
            {
               Context.Workspaces.FocusedWorkspace.NextLayoutEngine();
            }, fontname: FontName));
        }

        public override void Initialize()
        {
            _timer = new Timer(200);
            _timer.Elapsed += (s, e) => MarkDirty();
            _timer.Enabled = true;
        }
    }
}
