using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using workspacer.TitleBar;

namespace workspacer.Titlebar
{
    public class TitleBarPluginConfig
    {
        public bool HideTitleBars { get; set; }
        public List<TitleBarConfigItem> TitleBarConfigItems { get; set; }

        public TitleBarPluginConfig() : this(true)
        {
        }

        public TitleBarPluginConfig(bool hideTitleBars)
        {
            HideTitleBars = hideTitleBars;
            TitleBarConfigItems = new List<TitleBarConfigItem>();
        }

        public void SetWindowClass(string windowClass, bool hide = false)
        {
            TitleBarConfigItems.Add(new TitleBarConfigItem(window => window.Class == windowClass, hide));
        }

        public void SetWindowProcessName(string processName, bool hide = false)
        {
            TitleBarConfigItems.Add(new TitleBarConfigItem(window => window.ProcessName == processName, hide));
        }

        public void SetWindowTitle(string title, bool hide = false)
        {
            TitleBarConfigItems.Add(new TitleBarConfigItem(window => window.Title == title, hide));
        }

        public void SetWindowTitleMAtch(string match, bool hide = false)
        {
            var regex = new Regex(match);
            TitleBarConfigItems.Add(new TitleBarConfigItem(window => regex.IsMatch(window.Title), hide));
        }
    }
}