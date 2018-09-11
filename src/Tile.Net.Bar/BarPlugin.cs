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

        public void AfterConfig(IConfigContext context)
        {
            _bars = new List<BarForm>();


            
            Task.Run(() =>
            {
                foreach (var m in context.Workspaces.Monitors)
                {
                    var bar = new BarForm(m, _config);

                    var widgetContext = new BarWidgetContext(bar, m, context.Workspaces);

                    var left = _config.LeftWidgets();
                    InitializeWidgets(left, widgetContext);
                    var middle = _config.MiddleWidgets();
                    InitializeWidgets(middle, widgetContext);
                    var right = _config.RightWidgets();
                    InitializeWidgets(right, widgetContext);

                    bar.LeftWidgets = left;
                    bar.MiddleWidgets = middle;
                    bar.RightWidgets = right;

                    bar.Show();
                    _bars.Add(bar);
                }
                Application.EnableVisualStyles();

                _bars.ForEach(b => b.Redraw());

                while (true)
                {
                    Thread.Sleep(500);
                    Application.DoEvents();
                }
            });
        }

        private void InitializeWidgets(IEnumerable<IBarWidget> widgets, IBarWidgetContext context)
        {
            foreach (var w in widgets)
            {
                w.Initialize(context);
            }
        }
    }
}
