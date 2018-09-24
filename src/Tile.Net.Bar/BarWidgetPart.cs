using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar
{
    public class BarWidgetPart : IBarWidgetPart
    {
        public string Text { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
    }
}
