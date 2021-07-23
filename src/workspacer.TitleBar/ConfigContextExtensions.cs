using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using workspacer.Titlebar;

namespace workspacer.TitleBar
{
    public static class ConfigContextExtensions
    {
        public static TitleBarPlugin AddTitleBar(this IConfigContext context, TitleBarPluginConfig config)
        {
            config ??= new TitleBarPluginConfig();
            var plugin = new TitleBarPlugin(config);
            context.Plugins.RegisterPlugin(plugin);
            return plugin;
        }
    }
}
