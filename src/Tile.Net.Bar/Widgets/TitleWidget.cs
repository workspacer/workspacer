using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar.Widgets
{
    public class TitleWidget : BarWidgetBase
    {
        public Color MonitorHasFocusColor { get; set; } = Color.Yellow;

        public override IBarWidgetPart[] GetParts()
        {
            var window = Context.Monitor.Workspace.FocusedWindow ??
                         Context.Monitor.Workspace.Windows.FirstOrDefault(w => w.CanLayout);

            var isFocusedMonitor = Context.Workspaces.FocusedMonitor == Context.Monitor;
            var multipleMonitors = Context.Workspaces.Monitors.Count() > 1;
            var color = isFocusedMonitor && multipleMonitors ? MonitorHasFocusColor : null;

            if (window != null)
            {
                return Parts(Part(window.Title, color));
            } else
            {
                return Parts(Part("no windows", color));
            }
        }

        public override void Initialize()
        {
            Context.Workspaces.WindowAdded += Refresh;
            Context.Workspaces.WindowRemoved += Refresh;
            Context.Workspaces.WindowUpdated += Refresh;
        }

        private void Refresh(IWindow window, IWorkspace workspace)
        {
            if (workspace == Context.Monitor.Workspace)
            {
                Context.MarkDirty();
            }
        }
    }
}
