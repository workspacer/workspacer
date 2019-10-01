using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer
{
    public class SystemTrayManager : ISystemTrayManager
    {
        private ContextMenuStrip _strip;
        private NotifyIcon _icon;

        public SystemTrayManager()
        {
            _icon = new NotifyIcon();
            _strip = _icon.ContextMenuStrip = new ContextMenuStrip();
            _icon.Icon = Properties.Resources.logo;
            _icon.Visible = true;
        }

        public void AddToContextMenu(string text, Action handler)
        {
            _strip.Items.Add(text, null, (s, e) => handler());
        }

        public void Destroy()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            var icon = _icon;
            if (icon != null)
            {
                _icon = null;
                icon.Visible = false;
                icon.Dispose();
            }

            var strip = _strip;
            if (strip != null)
            {
                _strip = null;
                strip.Dispose();
            }
        }
    }
}
