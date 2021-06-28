using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public class BarWidgetPart : IBarWidgetPart
    {
        public string Text { get; set; }
        public string LeftPadding { get; set; }
        public string RightPadding { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Action PartClicked { get; set; }
        public string FontName { get; set; }
    }
}
