using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Workspacer.ConfigLoader;

namespace Workspacer
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

        public void Unsubscribe(MouseEvent evt)
        {
            if (_mouseSubs.ContainsKey(evt))
                _mouseSubs.Remove(evt);
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

        public void SubscribeDefaults(IConfigContext context, KeyModifiers mod)
        {
            Subscribe(MouseEvent.LButtonDown,
                () => context.Workspaces.SwitchFocusedMonitorToMouseLocation());

            Subscribe(mod | KeyModifiers.LShift, Keys.E,
                () => context.Enabled = !context.Enabled);

            Subscribe(mod | KeyModifiers.LShift, Keys.C,
                () => context.Workspaces.FocusedWorkspace.CloseFocusedWindow());

            Subscribe(mod, Keys.Space,
                () => context.Workspaces.FocusedWorkspace.NextLayoutEngine());

            Subscribe(mod | KeyModifiers.LShift, Keys.Space,
                () => context.Workspaces.FocusedWorkspace.PreviousLayoutEngine());

            Subscribe(mod, Keys.N,
                () => context.Workspaces.FocusedWorkspace.ResetLayout());

            Subscribe(mod, Keys.J,
                () => context.Workspaces.FocusedWorkspace.FocusNextWindow());

            Subscribe(mod, Keys.K,
                () => context.Workspaces.FocusedWorkspace.FocusPreviousWindow());

            Subscribe(mod, Keys.M,
                () => context.Workspaces.FocusedWorkspace.FocusPrimaryWindow());

            Subscribe(mod, Keys.Enter,
                () => context.Workspaces.FocusedWorkspace.SwapFocusAndPrimaryWindow());

            Subscribe(mod | KeyModifiers.LShift, Keys.J,
                () => context.Workspaces.FocusedWorkspace.SwapFocusAndNextWindow());

            Subscribe(mod | KeyModifiers.LShift, Keys.K,
                () => context.Workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow());

            Subscribe(mod, Keys.H,
                () => context.Workspaces.FocusedWorkspace.ShrinkPrimaryArea());

            Subscribe(mod, Keys.L,
                () => context.Workspaces.FocusedWorkspace.ExpandPrimaryArea());

            Subscribe(mod, Keys.Oemcomma,
                () => context.Workspaces.FocusedWorkspace.IncrementNumberOfPrimaryWindows());

            Subscribe(mod, Keys.OemPeriod,
                () => context.Workspaces.FocusedWorkspace.DecrementNumberOfPrimaryWindows());

            Subscribe(mod | KeyModifiers.LShift, Keys.Q, context.Quit);

            Subscribe(mod, Keys.Q, context.Restart);

            Subscribe(mod, Keys.D1,
                () => context.Workspaces.SwitchToWorkspace(0));

            Subscribe(mod, Keys.D2,
                () => context.Workspaces.SwitchToWorkspace(1));

            Subscribe(mod, Keys.D3,
                () => context.Workspaces.SwitchToWorkspace(2));

            Subscribe(mod, Keys.D4,
                () => context.Workspaces.SwitchToWorkspace(3));

            Subscribe(mod, Keys.D5,
                () => context.Workspaces.SwitchToWorkspace(4));

            Subscribe(mod, Keys.D6,
                () => context.Workspaces.SwitchToWorkspace(5));

            Subscribe(mod, Keys.D7,
                () => context.Workspaces.SwitchToWorkspace(6));

            Subscribe(mod, Keys.D8,
                () => context.Workspaces.SwitchToWorkspace(7));

            Subscribe(mod, Keys.D9,
                () => context.Workspaces.SwitchToWorkspace(8));

            Subscribe(mod, Keys.W,
                () => context.Workspaces.SwitchFocusedMonitor(0));

            Subscribe(mod, Keys.E,
                () => context.Workspaces.SwitchFocusedMonitor(1));

            Subscribe(mod, Keys.R,
                () => context.Workspaces.SwitchFocusedMonitor(2));

            Subscribe(mod | KeyModifiers.LShift, Keys.W,
                () => context.Workspaces.MoveFocusedWindowToMonitor(0));

            Subscribe(mod | KeyModifiers.LShift, Keys.E,
                () => context.Workspaces.MoveFocusedWindowToMonitor(1));

            Subscribe(mod | KeyModifiers.LShift, Keys.R,
                () => context.Workspaces.MoveFocusedWindowToMonitor(2));

            Subscribe(mod | KeyModifiers.LShift, Keys.D1,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(0));

            Subscribe(mod | KeyModifiers.LShift, Keys.D2,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(1));

            Subscribe(mod | KeyModifiers.LShift, Keys.D3,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(2));

            Subscribe(mod | KeyModifiers.LShift, Keys.D4,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(3));

            Subscribe(mod | KeyModifiers.LShift, Keys.D5,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(4));

            Subscribe(mod | KeyModifiers.LShift, Keys.D6,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(5));

            Subscribe(mod | KeyModifiers.LShift, Keys.D7,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(6));

            Subscribe(mod | KeyModifiers.LShift, Keys.D8,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(7));

            Subscribe(mod | KeyModifiers.LShift, Keys.D9,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(8));
        }
    }
}
