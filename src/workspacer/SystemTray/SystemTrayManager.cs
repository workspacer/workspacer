using System;
using System.Windows.Forms;

namespace workspacer
{
    public class SystemTrayManager : ISystemTrayManager
    {
        private readonly ContextMenuStrip _strip;
        private readonly NotifyIcon _icon;

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

        public void Dispose()
        {
            _icon.Visible = false;
            _icon.Dispose();
            _strip.Dispose();
        }
    }
}
