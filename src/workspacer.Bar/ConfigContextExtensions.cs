using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public static class ConfigContextExtensions
    {
        public static void AddBar(this IConfigContext context, BarPluginConfig config = null)
        {
            config = config ?? new BarPluginConfig();

            context.AddLayoutProxy((layout) => new MenuBarLayoutEngine(layout, config.BarTitle, config.BarHeight));

            context.Plugins.RegisterPlugin(new BarPlugin(config));
        }

        public static void AddBar(this IConfigContext context, Action<BarPluginConfig> onConfig)
        {
            var config = new BarPluginConfig();
            onConfig(config);
            context.AddBar(config);
        }
    }
}
