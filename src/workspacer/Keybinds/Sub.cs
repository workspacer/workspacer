using System.Collections.Generic;

namespace workspacer
{
    public class Sub
    {
        private KeyModifiers _mod;
        private Keys _key;

        public Sub(KeyModifiers mod, Keys key)
        {
            _mod = mod;
            _key = key;
        }

        public sealed class SubEqualityComparer : IEqualityComparer<Sub>
        {
            bool IEqualityComparer<Sub>.Equals(Sub x, Sub y)
            {
                if (((x._mod & KeyModifiers.Alt) != KeyModifiers.None ||
                     (y._mod & KeyModifiers.Alt) != KeyModifiers.None) &&
                    (x._mod & KeyModifiers.Alt & y._mod) == KeyModifiers.None)
                {
                    return false;
                }
                if (((x._mod & KeyModifiers.Control) != KeyModifiers.None ||
                     (y._mod & KeyModifiers.Control) != KeyModifiers.None) &&
                    (x._mod & KeyModifiers.Control & y._mod) == KeyModifiers.None)
                {
                    return false;
                }
                if (((x._mod & KeyModifiers.Shift) != KeyModifiers.None ||
                     (y._mod & KeyModifiers.Shift) != KeyModifiers.None) &&
                    (x._mod & KeyModifiers.Shift & y._mod) == KeyModifiers.None)
                {
                    return false;
                }
                if (((x._mod & KeyModifiers.Win) != KeyModifiers.None ||
                     (y._mod & KeyModifiers.Win) != KeyModifiers.None) &&
                    (x._mod & KeyModifiers.Win & y._mod) == KeyModifiers.None)
                {
                    return false;
                }
                return x._key == y._key;
            }

            int IEqualityComparer<Sub>.GetHashCode(Sub obj)
            {
                var modifiers = 0;
                if ((obj._mod & KeyModifiers.Alt) != KeyModifiers.None)
                {
                    modifiers += 1;
                }
                if ((obj._mod & KeyModifiers.Control) != KeyModifiers.None)
                {
                    modifiers += 2;
                }
                if ((obj._mod & KeyModifiers.Shift) != KeyModifiers.None)
                {
                    modifiers += 4;
                }
                if ((obj._mod & KeyModifiers.Win) != KeyModifiers.None)
                {
                    modifiers += 8;
                }

                return modifiers + 256 + (int)obj._key;
            }
        }
    }
}
