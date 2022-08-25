using System;

namespace workspacer.ActionMenu
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
