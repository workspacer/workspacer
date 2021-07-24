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

        private void SetStyle(IWindow window, TitleBarStyle settings)
        {
            var style = (Win32.WS)Win32.GetWindowLongPtr(window.Handle, Win32.GWL_STYLE);
            if (settings.ShowTitleBar)
            {
                style |= Win32.WS.WS_CAPTION;
            }
            else
            {
                style &= ~Win32.WS.WS_CAPTION;
            }

            if (settings.ShowSizingBorder)
            {
                style |= Win32.WS.WS_THICKFRAME;
            }
            else
            {
                style &= ~Win32.WS.WS_THICKFRAME;
            }
            Win32.SetWindowLongPtr(window.Handle, Win32.GWL_STYLE, (IntPtr)style);
        }

        private void WindowAdded(IWindow window, IWorkspace workspace)
        {
            var style = GetStyle(window);
            if (style != null)
            {
                SetStyle(window, style);
            }
        }

        private TitleBarStyle GetStyle(IWindow window)
        {
            foreach (var rule in _config.Rules)
            {
                if (rule.Matcher(window))
                {
                    return rule.Style;
                }
            }
            return null;
        }
    }
}