using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IKeybindManager
    {
        void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler);
        void Subscribe(MouseEvent evt, MouseHandler handler);
        bool KeyIsPressed(Keys key);
    }
}
