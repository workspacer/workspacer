using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile.Net.Bar
{
    public class BarPlugin : IPlugin
    {
        private BarForm _form;

        public void AfterConfig(IConfigContext context)
        {
            context.Workspaces.WorkspaceUpdated += () => UpdateWorkspaces(context);

            Task.Run(() =>
            {
                _form = new BarForm();
                Application.EnableVisualStyles();
                Application.Run(_form);
                UpdateWorkspaces(context);
                while (true)
                {
                    Application.DoEvents();
                }
            });
        }

        private void UpdateWorkspaces(IConfigContext context)
        {
            var parts = new List<string>();

            foreach (var w in context.Workspaces.Workspaces)
            {
                var monitor = context.Workspaces.GetMonitorForWorkspace(w);
                var part = monitor != null ? $"[{w.Name}]" : w.Name;
                parts.Add(part);
            }
            var full = string.Join(" ", parts);

            _form.SetLeft(full);
        }
    }
}
