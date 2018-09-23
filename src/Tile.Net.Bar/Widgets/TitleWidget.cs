using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar.Widgets
{
    public class TitleWidget : IBarWidget
    {
        private IBarWidgetContext _context;

        public string GetText()
        {
            var window = _context.Monitor.Workspace.FocusedWindow ??
                         _context.Monitor.Workspace.Windows.FirstOrDefault(w => w.CanLayout);

            if (window != null)
            {
                return window.Title;
            } else
            {
                return "";
            }
        }

        public void Initialize(IBarWidgetContext context)
        {
            _context = context;

            _context.Workspaces.WindowAdded += Refresh;
            _context.Workspaces.WindowRemoved += Refresh;
            _context.Workspaces.WindowUpdated += Refresh;
        }

        private void Refresh(IWindow window, IWorkspace workspace)
        {
            if (workspace == _context.Monitor.Workspace)
            {
                _context.MarkDirty();
            }
        }
    }
}
