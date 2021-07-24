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

        public void SetWindowClass(string windowClass, bool hide = false)
        {
            Rules.Add(new TitleBarRule(window => window.Class == windowClass, hide));
        }

        public void SetWindowProcessName(string processName, bool hide = false)
        {
            Rules.Add(new TitleBarRule(window => window.ProcessName == processName, hide));
        }

        public void SetWindowTitle(string title, bool hide = false)
        {
            Rules.Add(new TitleBarRule(window => window.Title == title, hide));
        }

        public void SetWindowTitleMAtch(string match, bool hide = false)
        {
            var regex = new Regex(match);
            Rules.Add(new TitleBarRule(window => regex.IsMatch(window.Title), hide));
        }
    }
}