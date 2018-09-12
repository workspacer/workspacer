using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar.Widgets
{
    public class WorkspaceWidget : IBarWidget
    {
        private IBarWidgetContext _context;

        public void Initialize(IBarWidgetContext context)
        {
            _context = context;

            context.Workspaces.WorkspaceUpdated += () => UpdateWorkspaces();
        }

        public string GetText()
        {
            var parts = new List<string>();
            var workspaces = _context.Workspaces.Workspaces;
            foreach (var workspace in workspaces)
            {
                if (workspace.Monitor == _context.Monitor)
                {
                    parts.Add($"[{workspace.Name}]");
                } else
                {
                    parts.Add($" {workspace.Name} ");
                }
            }
            return string.Join("", parts);
        }

        private void UpdateWorkspaces()
        {
            _context.MarkDirty();
        }
    }
}
