using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Gap
{
    using System.Runtime.CompilerServices;

    public static class ConfigContextExtensions
    {
        public static GapPlugin AddGap(this IConfigContext context, GapPluginConfig config)
        {
            config ??= new GapPluginConfig();
            var plugin = new GapPlugin(config);

            context.AddLayoutProxy((layout) =>
            {
                var gapLayout = new GapLayoutEngine(layout, config.InnerGap, config.OuterGap, config.Delta, config.OnFocused);
                plugin.RegisterLayout(gapLayout);
                return gapLayout;
            });
            context.Plugins.RegisterPlugin(plugin);

            return plugin;
        }

        public static GapPlugin AddGap(this IConfigContext context, Action<GapPluginConfig> onConfigure)
        {
            var config = new GapPluginConfig();
            onConfigure(config);
            return context.AddGap(config);
        }
    }
}
