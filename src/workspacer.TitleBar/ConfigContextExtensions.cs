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

        public static TitleBarPlugin AddTitleBar(this IConfigContext context, Action<TitleBarPluginConfig> onConfig)
        {
            var config = new TitleBarPluginConfig();
            onConfig(config);
            return context.AddTitleBar(config);
        }
    }
}
