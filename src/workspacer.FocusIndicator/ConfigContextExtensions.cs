namespace workspacer.FocusIndicator
{
    public static class ConfigContextExtensions
    {
        public static void AddFocusIndicator(this IConfigContext context, FocusIndicatorPluginConfig config = null)
        {
            context.Plugins.RegisterPlugin(new FocusIndicatorPlugin(config ?? new FocusIndicatorPluginConfig()));
        }
    }
}
