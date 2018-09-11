using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar
{
    public class BarPluginConfig
    {
        public int BarHeight { get; set; } = 30;
        public int FontSize { get; set; } = 16;
        public Func<IBarWidget[]> LeftWidgets { get; set; } = () => new IBarWidget[0];
        public Func<IBarWidget[]> MiddleWidgets { get; set; } = () => new IBarWidget[0];
        public Func<IBarWidget[]> RightWidgets { get; set; } = () => new IBarWidget[0];
    }
}
