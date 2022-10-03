using System;

namespace workspacer.ActionMenu
{
    public static class ConfigContextExtensions
    {
        public static ActionMenuPlugin AddActionMenu(this IConfigContext context, ActionMenuPluginConfig config = null)
        {
            return context.Plugins.RegisterPlugin(new ActionMenuPlugin(config ?? new ActionMenuPluginConfig()));
        }

        public static ActionMenuPlugin AddActionMenu(
            this IConfigContext context,
            Action<ActionMenuPluginConfig> configAction)
        {
            var config = new ActionMenuPluginConfig();
            configAction.Invoke(config);
            return AddActionMenu(context, config);
        }
    }
}
