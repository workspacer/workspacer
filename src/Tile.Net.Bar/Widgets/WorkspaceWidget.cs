using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar.Widgets
{
    public class WorkspaceWidget : BarWidgetBase
    {
        public override void Initialize()
        {
            Context.Workspaces.WorkspaceUpdated += () => UpdateWorkspaces();
            Context.Workspaces.WindowMoved += (w, o, n) => UpdateWorkspaces();
        }

        public override IBarWidgetPart[] GetParts()
        {
            var parts = new List<IBarWidgetPart>();
            var workspaces = Context.Workspaces.Workspaces;
            foreach (var workspace in workspaces)
            {
                var hasWindows = workspace.Windows.Any(w => w.CanLayout);

                if (workspace.Monitor == Context.Monitor)
                {
                    parts.Add(Part($"[{workspace.Name}]", Color.Red));
                } else
                {
                    parts.Add(Part($" {workspace.Name} ", hasWindows ? null : Color.Gray));
                }
            }
            return parts.ToArray();
        }

        private void UpdateWorkspaces()
        {
            Context.MarkDirty();
        }
    }
}
