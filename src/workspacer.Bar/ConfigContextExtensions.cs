using System;

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
        
        public static BarPlugin AddBar(
            this IConfigContext context,
            Action<BarPluginConfig> configAction)
        {
            var config = new BarPluginConfig();
            configAction.Invoke(config);
            return AddBar(context, config);
        }
    }
}
