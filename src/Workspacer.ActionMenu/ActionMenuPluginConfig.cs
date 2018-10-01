using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.ActionMenu
{
    public class ActionMenuPluginConfig
    {
        public string MenuTitle { get; set; } = "Workspacer.ActionMenu";
        public int MenuHeight { get; set; } = 50;
        public int MenuWidth { get; set; } = 500;
        public int FontSize { get; set; } = 16;

        public Color Background { get; set; } = Color.Black;
        public Color Foreground { get; set; } = Color.White;
    }
}
