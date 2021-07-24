using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.TitleBar
{
    public class TitleBarRule
    {
        public readonly bool ShowTitleBar;
        public readonly Func<IWindow, bool> Matcher;

        public TitleBarRule(Func<IWindow, bool> matcher, bool showTitleBar)
        {
            Matcher = matcher;
            ShowTitleBar = showTitleBar;
        }
    }
}
