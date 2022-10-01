using System;

namespace workspacer.FocusBorder
{
    public static class ConfigContextExtensions
    {
        public static FocusBorderPlugin AddFocusBorder(this IConfigContext context, FocusBorderPluginConfig config = null)
        {
            return context.Plugins.RegisterPlugin(new FocusBorderPlugin(config ?? new FocusBorderPluginConfig()));
        }

        public static FocusBorderPlugin AddFocusBorder(this IConfigContext context, Action<FocusBorderPluginConfig> onConfig)
        {
            var config = new FocusBorderPluginConfig();
            onConfig(config);
            return context.AddFocusBorder(config);
        }
    }
}
