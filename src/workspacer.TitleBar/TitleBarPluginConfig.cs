using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using workspacer.TitleBar;

namespace workspacer.TitleBar
{
    public class TitleBarPluginConfig
    {
        public bool ShowTitleBars { get; set; }
        public List<TitleBarRule> Rules { get; set; }

        public TitleBarPluginConfig() : this(false)
        {
        }

        public TitleBarPluginConfig(bool hideTitleBars)
        {
            ShowTitleBars = hideTitleBars;
            Rules = new List<TitleBarRule>();
        }

        public void SetWindowClass(string windowClass, TitleBarStyle style = null)
        {
            Rules.Add(new TitleBarRule(window => window.Class == windowClass, style));
        }

        public void SetWindowProcessName(string processName, TitleBarStyle style = null)
        {
            Rules.Add(new TitleBarRule(window => window.ProcessName == processName, style));
        }

        public void SetWindowTitle(string title, TitleBarStyle style = null)
        {
            Rules.Add(new TitleBarRule(window => window.Title == title, style));
        }

        public void SetWindowTitleMAtch(string match, TitleBarStyle style = null)
        {
            var regex = new Regex(match);
            Rules.Add(new TitleBarRule(window => regex.IsMatch(window.Title), style));
        }
    }
}