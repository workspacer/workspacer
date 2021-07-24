using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.TitleBar
{
    public class TitleBarPlugin : IPlugin
    {
        private TitleBarPluginConfig _config;
        private IConfigContext _context;

        public TitleBarPlugin()
        {
            _config = new TitleBarPluginConfig();
        }

        public TitleBarPlugin(TitleBarPluginConfig config)
        {
            _config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            _context = context;
            _context.Workspaces.WindowAdded += WindowAdded;
        }
        private void WindowAdded(IWindow window, IWorkspace workspace)
        {
            if (!ShowTitlebar(window))
            {
                var style = Win32.GetWindowLongPtr(window.Handle, Win32.GWL_STYLE);
                Win32.SetWindowStyleLongPtr(window.Handle, (Win32.WS)style & ~Win32.WS.WS_CAPTION);
            }
        }

        private bool ShowTitlebar(IWindow window)
        {
            foreach (var rule in _config.Rules)
            {
                if (rule.Matcher(window))
                {
                    return rule.ShowTitleBar;
                }
            }

            return _config.ShowTitleBars;
        }
    }
}