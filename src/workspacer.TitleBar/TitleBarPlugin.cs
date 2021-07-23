using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using workspacer.Titlebar;

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
        }
    }
}