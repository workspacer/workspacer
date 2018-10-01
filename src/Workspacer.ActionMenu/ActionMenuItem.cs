using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.ActionMenu
{
    public class ActionMenuItem
    {
        public string Text { get; set; }
        public Action Callback { get; set; }

        public ActionMenuItem(string text, Action callback)
        {
            Text = text;
            Callback = callback;
        }
    }
}
