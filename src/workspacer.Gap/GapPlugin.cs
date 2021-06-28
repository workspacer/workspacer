using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Gap
{
    public class GapPlugin : IPlugin
    {
        private GapPluginConfig _config;
        private IConfigContext _context;
        private List<GapLayoutEngine> _layoutEngines = new List<GapLayoutEngine>();

        public GapPlugin()
        {
            _config = new GapPluginConfig();
        }

        public GapPlugin(GapPluginConfig config)
        {
            _config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            _context = context;
        }

        public void RegisterLayout(GapLayoutEngine layout)
        {
            _layoutEngines.Add(layout);
        }

        public void IncrementInnerGap()
        {
            foreach (var layout in _layoutEngines)
            {
                layout.IncrementInnerGap();
            }
            DoWorkspaceLayouts();
        }

        public void DecrementInnerGap()
        {
            foreach (var layout in _layoutEngines)
            {
                layout.DecrementInnerGap();
            }
            DoWorkspaceLayouts();
        }

        public void IncrementOuterGap()
        {
            foreach (var layout in _layoutEngines)
            {
                layout.IncrementOuterGap();
            }
            DoWorkspaceLayouts();
        }

        public void DecrementOuterGap()
        {
            foreach (var layout in _layoutEngines)
            {
                layout.DecrementOuterGap();
            }
            DoWorkspaceLayouts();
        }

        public void ClearGaps()
        {
            foreach (var layout in _layoutEngines)
            {
                layout.ClearGaps();
            }
            DoWorkspaceLayouts();
        }

        private void DoWorkspaceLayouts()
        {
            foreach (var workspace in _context.WorkspaceContainer.GetAllWorkspaces())
            {
                workspace.DoLayout();
            }
        }
    }
}