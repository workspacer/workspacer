using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.ActionMenu
{
    public class ActionMenuItemBuilder
    {
        private List<ActionMenuItem> _items;
        private ActionMenuPlugin _plugin;

        internal ActionMenuItemBuilder(List<ActionMenuItem> items, ActionMenuPlugin plugin)
        {
            _items = items;
            _plugin = plugin;
        }

        internal ActionMenuItemBuilder(ActionMenuPlugin plugin)
        {
            _items = new List<ActionMenuItem>();
            _plugin = plugin;
        }

        public ActionMenuItemBuilder Add(string text, Action callback)
        {
            _items.Add(new ActionMenuItem(text, callback));
            return this;
        }

        public ActionMenuItemBuilder AddFreeForm(string text, Action<string> callback)
        {
            Add(text, () => _plugin.ShowFreeForm(text, callback));
            return this;
        }

        public ActionMenuItemBuilder AddMenu(string text, ActionMenuItemBuilder builder)
        {
            Add(text, () => _plugin.ShowMenu(text, builder));
            return this;
        }

        public ActionMenuItemBuilder AddMenu(string text, Func<ActionMenuItemBuilder> builder)
        {
            Add(text, () => _plugin.ShowMenu(text, builder()));
            return this;
        }

        public ActionMenuItem[] Get()
        {
            return _items.ToArray();
        }
    }
}
