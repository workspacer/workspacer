using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.Bar
{
    public interface IBarWidgetPart
    {
        string Text { get; }
        Color ForegroundColor { get; }
        Color BackgroundColor { get; }
        Action PartClicked { get; }
    }
}
