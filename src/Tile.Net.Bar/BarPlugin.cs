using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tile.Net.PluginInterface;

namespace Tile.Net.Bar
{
    public class BarPlugin : IPlugin
    {
        public void BeforeConfig()
        {
        }

        public void AfterConfig()
        {
            Task.Run(() =>
            {
                var form = new BarForm();
                Application.EnableVisualStyles();
                Application.Run(form);

                while (true)
                {
                    Application.DoEvents();
                }
            });
        }
    }
}
