using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public interface IBarWidgetPart
    {
        string Text { get; }
        string LeftPadding { get; }
        string RightPadding { get;  }
        Color ForegroundColor { get; }
        Color BackgroundColor { get; }
        Action PartClicked { get; }
        string FontName { get; }
    }
}
