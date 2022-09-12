using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public static class ConfigContextExtensions
    {
        public static void AddBar(this IConfigContext context, BarPluginConfig config = null)
        {
            config = config ?? new BarPluginConfig();

            context.AddLayoutProxy((layout) => new MenuBarLayoutEngine(layout, config));

            context.Plugins.RegisterPlugin(new BarPlugin(config));
        }

        public static void AddBar(this IConfigContext context, Action<BarPluginConfig> configAction)
        {
            var config = new BarPluginConfig();
            configAction.Invoke(config);
            context.AddBar(config);
        }
    }
}
