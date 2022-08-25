namespace workspacer.ActionMenu
{
    public static class ConfigContextExtensions
    {
        public static ActionMenuPlugin AddActionMenu(this IConfigContext context, ActionMenuPluginConfig config = null)
        {
            return context.Plugins.RegisterPlugin(new ActionMenuPlugin(config ?? new ActionMenuPluginConfig()));
        }

        public static ActionMenuPlugin AddActionMenu(this IConfigContext context, Action<ActionMenuPluginConfig> onConfig)
        {
            var config = new ActionMenuPluginConfig();
            onConfig(config);
            return context.AddActionMenu(config);
        }
    }
}
