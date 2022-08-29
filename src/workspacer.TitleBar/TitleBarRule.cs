using System;

namespace workspacer.TitleBar
{
    public class TitleBarRule
    {
        public readonly TitleBarStyle Style;
        public readonly Func<IWindow, bool> Matcher;

        public TitleBarRule(Func<IWindow, bool> matcher, TitleBarStyle style)
        {
            Matcher = matcher;
            Style = style;
        }
    }
}
