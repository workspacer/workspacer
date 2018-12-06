using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.ActionMenu
{
    public static class ConfigContextExtensions
    {
        public static ActionMenuPlugin AddActionMenu(this IConfigContext context, ActionMenuPluginConfig config = null)
        {
            return context.Plugins.RegisterPlugin(new ActionMenuPlugin(config ?? new ActionMenuPluginConfig()));
        }
    }
}
