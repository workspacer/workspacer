using System;

namespace workspacer.FocusIndicator
{
    public static class ConfigContextExtensions
    {
        public static FocusIndicatorPlugin AddFocusIndicator(this IConfigContext context, FocusIndicatorPluginConfig config = null)
        {
            return context.Plugins.RegisterPlugin(new FocusIndicatorPlugin(config ?? new FocusIndicatorPluginConfig()));
        }
        
        public static FocusIndicatorPlugin AddFocusIndicator(this IConfigContext context, Action<FocusIndicatorPluginConfig> onConfig)
        {
            var config = new FocusIndicatorPluginConfig();
            onConfig(config);
            return context.AddFocusIndicator(config);
        }
    }
}
