using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tile.Net.ConfigLoader;

namespace Tile.Net
{
    public partial class KeybindManager : IKeybindManager
    {
        private Win32.HookProc _hook;

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
             return (Win32.GetKeyState(KeysToKeys(key)) & 0x8000) == 0x8000;
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
                    if ((Win32.GetKeyState(KeysToKeys(Keys.LShiftKey)) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LShift;
                    }
                    if ((Win32.GetKeyState(KeysToKeys(Keys.RShiftKey)) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.RShift;
                    }
                    if ((Win32.GetKeyState(KeysToKeys(Keys.LMenu)) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LAlt;
                    }
                    if ((Win32.GetKeyState(KeysToKeys(Keys.RMenu)) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.RAlt;
                    }
                    if ((Win32.GetKeyState(KeysToKeys(Keys.LControlKey)) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LControl;
                    }
                    if ((Win32.GetKeyState(KeysToKeys(Keys.RControlKey)) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.RControl;
                    }
                    if ((Win32.GetKeyState(KeysToKeys(Keys.LWin)) & 0x8000) == 0x8000)
                    {
                        modifiersPressed |= KeyModifiers.LWin;
                    }
                    if ((Win32.GetKeyState(KeysToKeys(Keys.RWin)) & 0x8000) == 0x8000)
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
        
        private System.Windows.Forms.Keys KeysToKeys(Keys keys)
        {
            return (System.Windows.Forms.Keys)keys;
        }
    }
}
