using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface IKeybindManager
    {
        void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler);
        void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler, string name);
        void Subscribe(MouseEvent evt, MouseHandler handler);
        void Subscribe(MouseEvent evt, MouseHandler handler, string name);
        void Unsubscribe(MouseEvent evt);

        bool KeyIsPressed(Keys key);
        void UnsubscribeAll();
    }
}
