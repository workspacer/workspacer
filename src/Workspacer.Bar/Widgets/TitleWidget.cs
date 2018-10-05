using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.Bar.Widgets
{
    public class TitleWidget : BarWidgetBase
    {
        public Color MonitorHasFocusColor { get; set; } = Color.Yellow;

        public override IBarWidgetPart[] GetParts()
        {
            var window = GetWindow();
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
            Context.Workspaces.WindowAdded += RefreshAddRemove;
            Context.Workspaces.WindowRemoved += RefreshAddRemove;
            Context.Workspaces.WindowUpdated += RefreshUpdated;
            Context.Workspaces.FocusedMonitorUpdated += RefreshFocusedMonitor;
        }

        private IWindow GetWindow()
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            return currentWorkspace.FocusedWindow ??
                   currentWorkspace.LastFocusedWindow ??
                   currentWorkspace.Windows.FirstOrDefault(w => w.CanLayout);
        }

        private void RefreshAddRemove(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace)
            {
                Context.MarkDirty();
            }
        }

        private void RefreshUpdated(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && window == GetWindow())
            {
                Context.MarkDirty();
            }
        }

        private void RefreshFocusedMonitor()
        {
            Context.MarkDirty();
        }
    }
}
