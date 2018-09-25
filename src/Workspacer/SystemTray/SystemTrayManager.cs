using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workspacer
{
    public class SystemTrayManager : ISystemTrayManager
    {
        private ContextMenuStrip _strip;
        private NotifyIcon _icon;

        private SystemTrayManager()
        {
            _icon = new NotifyIcon();
            _strip = _icon.ContextMenuStrip = new ContextMenuStrip();
            _icon.Icon = Properties.Resources.logo;
            _icon.Visible = true;
        }

        public static SystemTrayManager Instance { get; } = new SystemTrayManager();

        public void AddToContextMenu(string text, Action handler)
        {
            _strip.Items.Add(text, null, (s, e) => handler());
        }
    }
}
