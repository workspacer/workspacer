using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace workspacer
{

    public partial class KeybindManager : IKeybindManager
    {
        
        private Logger Logger = Logger.Create();
        private Win32.HookProc _kbdHook;
        private Win32.HookProc _mouseHook;
        private IDictionary<Sub, NamedBind<KeybindHandler>> _kbdSubs;
        private IDictionary<MouseEvent, NamedBind<MouseHandler>> _mouseSubs;
        private string _activeMode;
        private TextBlockMessage _keybindDialog;
        private TextBlockMessage _keybindWarning;

        public KeybindManager()
        {
            _kbdHook = KbdHook;
            _mouseHook = MouseHook;
            _kbdSubs = new Dictionary<Sub, NamedBind<KeybindHandler>>();
            _mouseSubs = new Dictionary<MouseEvent, NamedBind<MouseHandler>>();

            var thread = new Thread(() =>
            {
                Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, _kbdHook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
                Win32.SetWindowsHookEx(Win32.WH_MOUSE_LL, _mouseHook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
                Application.Run();
            });
            thread.Name = "KeybindManager";
            thread.Start();

        }

        public void SetMode(KeyMode mode)
        {
            _kbdSubs = mode.KeyboardBindings;
            _mouseSubs = mode.MouseBindings;
            _activeMode = mode.Name;
        }


        public string GetModeName()
        {
            return _activeMode;
        }

        
       
        public IEnumerable<Tuple<KeyModifiers, Keys, string>> Keybinds => _kbdSubs.Select(kv => new Tuple<KeyModifiers, Keys, string>(kv.Key.Modifiers, kv.Key.Keys, kv.Value.Name));
        public IEnumerable<Tuple<MouseEvent, string>> Mousebinds => _mouseSubs.Select(kv => new Tuple<MouseEvent, string>(kv.Key, kv.Value.Name));

        public bool KeyIsPressed(Keys key)
        {
             return (Win32.GetKeyState(KeysToKeys(key)) & 0x8000) == 0x8000;
        }

        private IntPtr KbdHook(int nCode, UIntPtr wParam, IntPtr lParam)
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
            return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private IntPtr MouseHook(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode == 0 && (uint)wParam == Win32.WM_LBUTTONDOWN) { if (DoMouseEvent(MouseEvent.LButtonDown)) return new IntPtr(1); }
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
                    _kbdSubs[sub]?.Binding.Invoke();
                    return true;
                }
            }
            return false;
        }

        private bool DoMouseEvent(MouseEvent evt)
        {
            if (_mouseSubs.ContainsKey(evt))
            {
                _mouseSubs[evt]?.Binding.Invoke();
            }
            return false;
        }

        private System.Windows.Forms.Keys KeysToKeys(Keys keys)
        {
            return (System.Windows.Forms.Keys)keys;
        }

        private string GetKeybindString(KeyModifiers mods, Keys keys)
        {
            var parts = new List<string>();

            if (mods.HasFlag(KeyModifiers.LAlt))
                parts.Add("alt");
            if (mods.HasFlag(KeyModifiers.LShift))
                parts.Add("shift");

            if (keys == Keys.Oemcomma) { parts.Add(","); }
            else if (keys == Keys.OemPeriod) { parts.Add("."); }
            else if (keys == Keys.Oem2) { parts.Add("/"); }
            else if (new Regex("d\\d").IsMatch(keys.ToString().ToLower()))
            {
                parts.Add(keys.ToString().ToLower()[1].ToString());
            }
            else
            {
                parts.Add(keys.ToString().ToLower());
            }

            return string.Join("-", parts);
        }

        public void ShowKeybindDialog()
        {
            if (_keybindDialog == null)
            {
                var message = string.Join("\r\n", this.Keybinds.Select(k => (k.Item3 ?? "<unnamed>") + "  -  " + GetKeybindString(k.Item1, k.Item2)));
                _keybindDialog = new TextBlockMessage("workspacer keybinds", "below is the list of the current keybindings", message, new List<Tuple<string, Action>>()
                
                {
                    new Tuple<string, Action>("ok", () => { }),
                });
            } 

            if (_keybindDialog.Visible)
            {
                _keybindDialog.Hide();
            } else
            {
                _keybindDialog.Show();
            }
        }
    }
}
