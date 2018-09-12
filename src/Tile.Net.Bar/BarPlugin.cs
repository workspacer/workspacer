using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile.Net.Bar
{
    public class BarPlugin : IPlugin
    {
        private List<BarForm> _bars;
        private BarPluginConfig _config;

        public BarPlugin(BarPluginConfig config)
        {
            _config = config;
        }

        private class MyAppContext : ApplicationContext
        {
            public MyAppContext(BarPluginConfig config, IConfigContext context)
            {
                var bars = new List<BarForm>();

                foreach (var m in context.Workspaces.Monitors)
                {
                    var bar = new BarForm(m, config);

                    var widgetContext = new BarWidgetContext(bar, m, context.Workspaces);

                    var left = config.LeftWidgets();
                    InitializeWidgets(left, widgetContext);
                    var middle = config.MiddleWidgets();
                    InitializeWidgets(middle, widgetContext);
                    var right = config.RightWidgets();
                    InitializeWidgets(right, widgetContext);

                    bar.LeftWidgets = left;
                    bar.MiddleWidgets = middle;
                    bar.RightWidgets = right;

                    bar.Show();
                    bars.Add(bar);
                }
                bars.ForEach(b => b.MarkDirty());
            }

            private void InitializeWidgets(IEnumerable<IBarWidget> widgets, IBarWidgetContext context)
            {
                foreach (var w in widgets)
                {
                    w.Initialize(context);
                }
            }
        }

        public void AfterConfig(IConfigContext context)
        {
            Task.Run(() =>
            {
                Application.EnableVisualStyles();
                Application.Run(new MyAppContext(_config, context));
            });
        }
    }
}
