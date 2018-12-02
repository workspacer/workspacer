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
        void Subscribe(MouseEvent evt, MouseHandler handler);
        void Unsubscribe(MouseEvent evt);

        bool KeyIsPressed(Keys key);
        void UnsubscribeAll();
    }
}
