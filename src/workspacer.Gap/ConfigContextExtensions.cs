namespace workspacer.Gap
{
    public static class ConfigContextExtensions
    {
        public static GapPlugin AddGap(this IConfigContext context, GapPluginConfig config)
        {
            config ??= new GapPluginConfig();
            var plugin = new GapPlugin(config);

            context.AddLayoutProxy((layout) =>
            {
                var gapLayout = new GapLayoutEngine(layout, config.InnerGap, config.OuterGap, config.Delta);
                plugin.RegisterLayout(gapLayout);
                return gapLayout;
            });
            context.Plugins.RegisterPlugin(plugin);

            return plugin;
        }
    }
}
