using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public static class ConfigContextExtensions
    {
        public static BarPlugin AddBar(this IConfigContext context, BarPluginConfig config = null)
        {
            config = config ?? new BarPluginConfig();

            context.AddLayoutProxy((layout) => new MenuBarLayoutEngine(layout, config.BarTitle, config.BarHeight));

            return context.Plugins.RegisterPlugin(new BarPlugin(config));
        }
    }
}
