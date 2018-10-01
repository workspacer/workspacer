using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workspacer.ActionMenu
{
    public class ActionMenuPlugin : IPlugin
    {
        private ActionMenuForm _menu;
        private ActionMenuPluginConfig _config;

        public ActionMenuPlugin(ActionMenuPluginConfig config)
        {
            _config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            _menu = new ActionMenuForm(context, _config);
        }

        public void ShowMenu(ActionMenuItem[] items)
        {
            _menu.Items = items;
            _menu.Show();
            _menu.Activate();
        }

        public ActionMenuItemBuilder CreateDefault(IConfigContext context)
        {
            return new ActionMenuItemBuilder()
                .Add("restart workspacer", () => context.Restart())
                .Add("quit workspacer", () => context.Quit());
        }

        public ActionMenuItemBuilder Create()
        {
            return new ActionMenuItemBuilder();
        }
    }
}
