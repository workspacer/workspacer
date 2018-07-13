using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile.Net
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,

        LControl = 1,
        RControl = 2,
        Control = LControl | RControl,

        LShift = 4,
        RShift = 8,
        Shift = LShift | RShift,

        LAlt = 16,
        RAlt = 32,
        Alt = LAlt | RAlt,

        LWin = 64,
        RWin = 128,
        Win = LWin | RWin
    }

    public class KeybindManager
    {
        private class Sub
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

        private Win32.HookProc _hook;

        public delegate void KeybindHandler();
        private IDictionary<Sub, KeybindHandler> _subscriptions;

        private KeybindManager()
        {
            _hook = KeyboardHook;
            Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, _hook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);

            _subscriptions = new Dictionary<Sub, KeybindHandler>(new Sub.SubEqualityComparer());
        }
        public static KeybindManager Instance { get; } = new KeybindManager();

        public void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler)
        {
            var sub = new Sub(mod, key);
            if (_subscriptions.ContainsKey(sub))
            {
                _subscriptions[sub] += handler;
            }
            else
            {
                _subscriptions[sub] = handler;
            }
        }

        public bool KeyIsPressed(Keys key)
        {
             return (Win32.GetKeyState(key) & 0x8000) == 0x8000;
        }

        private IntPtr KeyboardHook(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode == 0 && ((uint)wParam == Win32.WM_KEYDOWN || (uint)wParam == Win32.WM_SYSKEYDOWN))
            {
                var key = (Keys) Marshal.ReadInt32(lParam);
                if (key != Keys.LShiftKey && key != Keys.RShiftKey &&
                    key != Keys.LMenu && key != Keys.RMenu &&
                    key != Keys.LControlKey && key != Keys.RControlKey &&
                    key != Keys.LWin && key != Keys.RWin)
                {
                    KeyModifiers modifiersPressed = 0;
                    // there is no other way to distinguish between left and right modifier keys
                    if ((Win32.GetKeyState(Keys.LShiftKey) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LShift;
                    }
                    if ((Win32.GetKeyState(Keys.RShiftKey) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.RShift;
                    }
                    if ((Win32.GetKeyState(Keys.LMenu) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LAlt;
                    }
                    if ((Win32.GetKeyState(Keys.RMenu) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.RAlt;
                    }
                    if ((Win32.GetKeyState(Keys.LControlKey) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LControl;
                    }
                    if ((Win32.GetKeyState(Keys.RControlKey) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.RControl;
                    }
                    if ((Win32.GetKeyState(Keys.LWin) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LWin;
                    }
                    if ((Win32.GetKeyState(Keys.RWin) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.RWin;
                    }

                    if (modifiersPressed != KeyModifiers.None)
                    {
                        var sub = new Sub(modifiersPressed, key);
                        if (_subscriptions.ContainsKey(sub))
                        {
                            _subscriptions[sub]?.Invoke();
                            return new IntPtr(1);
                        }
                    }
                }
            }

            return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
    }
}
