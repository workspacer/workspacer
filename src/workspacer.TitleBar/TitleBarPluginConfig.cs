using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace workspacer.TitleBar
{
    public class TitleBarPluginConfig
    {
        public TitleBarStyle DefaultStyle { get; set; }
        public List<TitleBarRule> Rules { get; set; }

        public TitleBarPluginConfig() : this(new TitleBarStyle(true, true))
        {
        }

        public TitleBarPluginConfig(TitleBarStyle defaultStyle)
        {
            DefaultStyle = defaultStyle;
            Rules = new List<TitleBarRule>();
        }

        public void SetWindowClass(string windowClass, TitleBarStyle style)
        {
            Rules.Add(new TitleBarRule(window => window.Class == windowClass, style));
        }

        public void SetWindowProcessName(string processName, TitleBarStyle style)
        {
            Rules.Add(new TitleBarRule(window => window.ProcessName == processName, style));
        }

        public void SetWindowTitle(string title, TitleBarStyle style)
        {
            Rules.Add(new TitleBarRule(window => window.Title == title, style));
        }

        public void SetWindowTitleMAtch(string match, TitleBarStyle style)
        {
            var regex = new Regex(match);
            Rules.Add(new TitleBarRule(window => regex.IsMatch(window.Title), style));
        }
    }
}