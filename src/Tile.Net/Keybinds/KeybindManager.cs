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

        private IDictionary<Sub, KeybindHandler> _kbdSubs;
        private IDictionary<MouseEvent, MouseHandler> _mouseSubs;

        private KeybindManager()
        {
            _hook = Hook;
            Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, _hook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
            Win32.SetWindowsHookEx(Win32.WH_MOUSE_LL, _hook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);

            _kbdSubs = new Dictionary<Sub, KeybindHandler>(new Sub.SubEqualityComparer());
            _mouseSubs = new Dictionary<MouseEvent, MouseHandler>();
        }
        public static KeybindManager Instance { get; } = new KeybindManager();

        public void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler)
        {
            var sub = new Sub(mod, key);
            if (_kbdSubs.ContainsKey(sub))
            {
                _kbdSubs[sub] += handler;
            }
            else
            {
                _kbdSubs[sub] = handler;
            }
        }

        public void Subscribe(MouseEvent evt, MouseHandler handler)
        {
            if (_mouseSubs.ContainsKey(evt))
            {
                _mouseSubs[evt] += handler;
            }
            else
            {
                _mouseSubs[evt] = handler;
            }
        }

        public bool KeyIsPressed(Keys key)
        {
             return (Win32.GetKeyState(KeysToKeys(key)) & 0x8000) == 0x8000;
        }

        private IntPtr Hook(int nCode, UIntPtr wParam, IntPtr lParam)
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

                    if (DoKeyboardEvent(key, modifiersPressed))
                    {
                        return new IntPtr(1);
                    }
                }
            }
            else if (nCode == 0 && (uint)wParam == Win32.WM_LBUTTONDOWN) { if (DoMouseEvent(MouseEvent.LButtonDown)) return new IntPtr(1); }
            else if (nCode == 0 && (uint)wParam == Win32.WM_LBUTTONUP) { if (DoMouseEvent(MouseEvent.LButtonUp)) return new IntPtr(1); }
            else if (nCode == 0 && (uint)wParam == Win32.WM_MOUSEMOVE) { if (DoMouseEvent(MouseEvent.MouseMove)) return new IntPtr(1); }
            else if (nCode == 0 && (uint)wParam == Win32.WM_MOUSEWHEEL) { if (DoMouseEvent(MouseEvent.MouseWheel)) return new IntPtr(1); }
            else if (nCode == 0 && (uint)wParam == Win32.WM_MOUSEHWHEEL) { if (DoMouseEvent(MouseEvent.MouseHWheel)) return new IntPtr(1); }
            else if (nCode == 0 && (uint)wParam == Win32.WM_RBUTTONDOWN) { if (DoMouseEvent(MouseEvent.RButtonDown)) return new IntPtr(1); }
            else if (nCode == 0 && (uint)wParam == Win32.WM_RBUTTONUP) { if (DoMouseEvent(MouseEvent.RButtonUp)) return new IntPtr(1); }

            return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private bool DoKeyboardEvent(Keys key, KeyModifiers modifiersPressed)
        {
            if (modifiersPressed != KeyModifiers.None)
            {
                var sub = new Sub(modifiersPressed, key);
                if (_kbdSubs.ContainsKey(sub))
                {
                    _kbdSubs[sub]?.Invoke();
                    return true;
                }
            }
            return false;
        }

        private bool DoMouseEvent(MouseEvent evt)
        {
            if (_mouseSubs.ContainsKey(evt))
            {
                _mouseSubs[evt]?.Invoke();
            }
            return false;
        }

        private System.Windows.Forms.Keys KeysToKeys(Keys keys)
        {
            return (System.Windows.Forms.Keys)keys;
        }
    }
}
