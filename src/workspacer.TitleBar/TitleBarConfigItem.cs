using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.TitleBar
{
    public class TitleBarConfigItem
    {
        public bool HideTitleBar { get; set; }
        public Func<IWindow, bool> Matcher { get; set; }

        public TitleBarConfigItem(Func<IWindow, bool> matcher, bool hideTitleBar)
        {
            matcher = Matcher;
            hideTitleBar = HideTitleBar;
        }
    }
}
