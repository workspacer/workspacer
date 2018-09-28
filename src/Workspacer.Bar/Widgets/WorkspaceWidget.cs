using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.Bar.Widgets
{
    public class WorkspaceWidget : BarWidgetBase
    {
        public Color WorkspaceHasFocusColor { get; set; } = Color.Red;
        public Color WorkspaceEmptyColor { get; set; } = Color.Gray;

        public override void Initialize()
        {
            Context.Workspaces.WorkspaceUpdated += () => UpdateWorkspaces();
            Context.Workspaces.WindowMoved += (w, o, n) => UpdateWorkspaces();
        }

        public override IBarWidgetPart[] GetParts()
        {
            var parts = new List<IBarWidgetPart>();
            var workspaces = Context.Workspaces.Container.GetWorkspaces(Context.Monitor);
            int index = 0;
            foreach (var workspace in workspaces)
            {
                var hasWindows = workspace.Windows.Any(w => w.CanLayout);

                var monitor = Context.Workspaces.Container.GetCurrentMonitorForWorkspace(workspace);
                if (monitor == Context.Monitor)
                {
                    parts.Add(CreatePart(workspace, index, true, WorkspaceHasFocusColor));
                } else
                {
                    parts.Add(CreatePart(workspace, index, false, hasWindows ? null : WorkspaceEmptyColor));
                }
                index++;
            }
            return parts.ToArray();
        }

        private IBarWidgetPart CreatePart(IWorkspace workspace, int index, bool visible, Color color)
        {
            return Part(GetDisplayName(workspace, index, visible), color, null, () =>
            {
                var monitorIndex = Context.Workspaces.Monitors.ToList().IndexOf(Context.Monitor);
                Context.Workspaces.SwitchMonitorToWorkspace(monitorIndex, index);
            });
        }

        private void UpdateWorkspaces()
        {
            Context.MarkDirty();
        }

        protected string GetDisplayName(IWorkspace workspace, int index, bool visible)
        {
            return visible ? $"[{workspace.Name}]" : $" {workspace.Name} ";
        }
    }
}
