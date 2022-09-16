using System;

namespace workspacer.FocusBorder
{
    public static class ConfigContextExtensions
    {
        public static void AddFocusBorder(this IConfigContext context, FocusBorderPluginConfig config = null)
        {
            context.Plugins.RegisterPlugin(new FocusBorderPlugin(config ?? new FocusBorderPluginConfig()));
        }

        public static void AddFocusBorder(this IConfigContext context, Action<FocusBorderPluginConfig> onConfig)
        {
            var config = new FocusBorderPluginConfig();
            onConfig(config);
            context.AddFocusBorder(config);
        }
    }
}
