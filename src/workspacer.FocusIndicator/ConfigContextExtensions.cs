using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.FocusIndicator
{
    public static class ConfigContextExtensions
    {
        public static FocusIndicatorPlugin AddFocusIndicator(this IConfigContext context, FocusIndicatorPluginConfig config = null)
        {
            return context.Plugins.RegisterPlugin(new FocusIndicatorPlugin(config ?? new FocusIndicatorPluginConfig()));
        }
    }
}
