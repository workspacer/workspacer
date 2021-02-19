using System.Collections.Generic;

namespace workspacer
{
    public class Sub
    {
        public KeyModifiers Modifiers { get; private set; }
        public Keys Keys { get; private set; }

        public Sub(KeyModifiers mod, Keys key)
        {
            Modifiers = mod;
            Keys = key;
        }

        public override bool Equals(object obj)
        {
            var x = this;
            var y = obj as Sub;
            if (y == null)
            {
                return false;
            }

            if (((x.Modifiers & KeyModifiers.Alt) != KeyModifiers.None ||
                 (y.Modifiers & KeyModifiers.Alt) != KeyModifiers.None) &&
                (x.Modifiers & KeyModifiers.Alt & y.Modifiers) == KeyModifiers.None)
            {
                return false;
            }
            if (((x.Modifiers & KeyModifiers.Control) != KeyModifiers.None ||
                 (y.Modifiers & KeyModifiers.Control) != KeyModifiers.None) &&
                (x.Modifiers & KeyModifiers.Control & y.Modifiers) == KeyModifiers.None)
            {
                return false;
            }
            if (((x.Modifiers & KeyModifiers.Shift) != KeyModifiers.None ||
                 (y.Modifiers & KeyModifiers.Shift) != KeyModifiers.None) &&
                (x.Modifiers & KeyModifiers.Shift & y.Modifiers) == KeyModifiers.None)
            {
                return false;
            }
            if (((x.Modifiers & KeyModifiers.Win) != KeyModifiers.None ||
                 (y.Modifiers & KeyModifiers.Win) != KeyModifiers.None) &&
                (x.Modifiers & KeyModifiers.Win & y.Modifiers) == KeyModifiers.None)
            {
                return false;
            }
            return x.Keys == y.Keys;
        }

        public override int GetHashCode()
        {
            var obj = this;
            var modifiers = 0;
            if ((obj.Modifiers & KeyModifiers.Alt) != KeyModifiers.None)
            {
                modifiers += 1;
            }
            if ((obj.Modifiers & KeyModifiers.Control) != KeyModifiers.None)
            {
                modifiers += 2;
            }
            if ((obj.Modifiers & KeyModifiers.Shift) != KeyModifiers.None)
            {
                modifiers += 4;
            }
            if ((obj.Modifiers & KeyModifiers.Win) != KeyModifiers.None)
            {
                modifiers += 8;
            }

            return modifiers + 256 * (int)obj.Keys;
        }
    }
}
