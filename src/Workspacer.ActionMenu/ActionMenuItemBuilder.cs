using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.ActionMenu
{
    public class ActionMenuItemBuilder
    {
        private List<ActionMenuItem> _items;

        internal ActionMenuItemBuilder(List<ActionMenuItem> items)
        {
            _items = items;
        }

        internal ActionMenuItemBuilder()
        {
            _items = new List<ActionMenuItem>();
        }

        public ActionMenuItemBuilder Add(string text, Action callback)
        {
            _items.Add(new ActionMenuItem(text, callback));
            return this;
        }

        public ActionMenuItem[] Get()
        {
            return _items.ToArray();
        }
    }
}
