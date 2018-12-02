using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using workspacer.ConfigLoader;

namespace workspacer
{
    public partial class KeybindManager : IKeybindManager
    {
        private Logger Logger = Logger.Create();

        private Win32.HookProc _kbdHook;
        private Win32.HookProc _mouseHook;

        private IConfigContext _context;
        private IDictionary<Sub, KeybindHandler> _kbdSubs;
        private IDictionary<MouseEvent, MouseHandler> _mouseSubs;

        public KeybindManager(IConfigContext context)
        {
            _context = context;
            _kbdHook = KbdHook;
            _mouseHook = MouseHook;
            _kbdSubs = new Dictionary<Sub, KeybindHandler>(new Sub.SubEqualityComparer());
            _mouseSubs = new Dictionary<MouseEvent, MouseHandler>();

            SubscribeDefaults();

            var thread = new Thread(() =>
            {
                Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, _kbdHook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
                Win32.SetWindowsHookEx(Win32.WH_MOUSE_LL, _mouseHook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
                Application.Run();
            });
            thread.Name = "KeybindManager";
            thread.Start();
        }

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

        public void UnsubscribeAll()
        {
            _kbdSubs.Clear();
            _mouseSubs.Clear();
        }

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

        public void SubscribeDefaults(KeyModifiers mod = KeyModifiers.LAlt)
        {
            Subscribe(MouseEvent.LButtonDown,
                () => _context.Workspaces.SwitchFocusedMonitorToMouseLocation());

            Subscribe(mod | KeyModifiers.LShift, Keys.E,
                () => _context.Enabled = !_context.Enabled);

            Subscribe(mod | KeyModifiers.LShift, Keys.C,
                () => _context.Workspaces.FocusedWorkspace.CloseFocusedWindow());

            Subscribe(mod, Keys.Space,
                () => _context.Workspaces.FocusedWorkspace.NextLayoutEngine());

            Subscribe(mod | KeyModifiers.LShift, Keys.Space,
                () => _context.Workspaces.FocusedWorkspace.PreviousLayoutEngine());

            Subscribe(mod, Keys.N,
                () => _context.Workspaces.FocusedWorkspace.ResetLayout());

            Subscribe(mod, Keys.J,
                () => _context.Workspaces.FocusedWorkspace.FocusNextWindow());

            Subscribe(mod, Keys.K,
                () => _context.Workspaces.FocusedWorkspace.FocusPreviousWindow());

            Subscribe(mod, Keys.M,
                () => _context.Workspaces.FocusedWorkspace.FocusPrimaryWindow());

            Subscribe(mod, Keys.Enter,
                () => _context.Workspaces.FocusedWorkspace.SwapFocusAndPrimaryWindow());

            Subscribe(mod | KeyModifiers.LShift, Keys.J,
                () => _context.Workspaces.FocusedWorkspace.SwapFocusAndNextWindow());

            Subscribe(mod | KeyModifiers.LShift, Keys.K,
                () => _context.Workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow());

            Subscribe(mod, Keys.H,
                () => _context.Workspaces.FocusedWorkspace.ShrinkPrimaryArea());

            Subscribe(mod, Keys.L,
                () => _context.Workspaces.FocusedWorkspace.ExpandPrimaryArea());

            Subscribe(mod, Keys.Oemcomma,
                () => _context.Workspaces.FocusedWorkspace.IncrementNumberOfPrimaryWindows());

            Subscribe(mod, Keys.OemPeriod,
                () => _context.Workspaces.FocusedWorkspace.DecrementNumberOfPrimaryWindows());

            Subscribe(mod, Keys.T,
                () => _context.Windows.ToggleFocusedWindowTiling());

            Subscribe(mod | KeyModifiers.LShift, Keys.Q, _context.Quit);

            Subscribe(mod, Keys.Q, _context.Restart);

            Subscribe(mod, Keys.D1,
                () => _context.Workspaces.SwitchToWorkspace(0));

            Subscribe(mod, Keys.D2,
                () => _context.Workspaces.SwitchToWorkspace(1));

            Subscribe(mod, Keys.D3,
                () => _context.Workspaces.SwitchToWorkspace(2));

            Subscribe(mod, Keys.D4,
                () => _context.Workspaces.SwitchToWorkspace(3));

            Subscribe(mod, Keys.D5,
                () => _context.Workspaces.SwitchToWorkspace(4));

            Subscribe(mod, Keys.D6,
                () => _context.Workspaces.SwitchToWorkspace(5));

            Subscribe(mod, Keys.D7,
                () => _context.Workspaces.SwitchToWorkspace(6));

            Subscribe(mod, Keys.D8,
                () => _context.Workspaces.SwitchToWorkspace(7));

            Subscribe(mod, Keys.D9,
                () => _context.Workspaces.SwitchToWorkspace(8));

            Subscribe(mod, Keys.Left,
                () => _context.Workspaces.SwitchToPreviousWorkspace());

            Subscribe(mod, Keys.Right,
                () => _context.Workspaces.SwitchToNextWorkspace());

            Subscribe(mod, Keys.W,
                () => _context.Workspaces.SwitchFocusedMonitor(0));

            Subscribe(mod, Keys.E,
                () => _context.Workspaces.SwitchFocusedMonitor(1));

            Subscribe(mod, Keys.R,
                () => _context.Workspaces.SwitchFocusedMonitor(2));

            Subscribe(mod | KeyModifiers.LShift, Keys.W,
                () => _context.Workspaces.MoveFocusedWindowToMonitor(0));

            Subscribe(mod | KeyModifiers.LShift, Keys.E,
                () => _context.Workspaces.MoveFocusedWindowToMonitor(1));

            Subscribe(mod | KeyModifiers.LShift, Keys.R,
                () => _context.Workspaces.MoveFocusedWindowToMonitor(2));

            Subscribe(mod | KeyModifiers.LShift, Keys.D1,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(0));

            Subscribe(mod | KeyModifiers.LShift, Keys.D2,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(1));

            Subscribe(mod | KeyModifiers.LShift, Keys.D3,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(2));

            Subscribe(mod | KeyModifiers.LShift, Keys.D4,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(3));

            Subscribe(mod | KeyModifiers.LShift, Keys.D5,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(4));

            Subscribe(mod | KeyModifiers.LShift, Keys.D6,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(5));

            Subscribe(mod | KeyModifiers.LShift, Keys.D7,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(6));

            Subscribe(mod | KeyModifiers.LShift, Keys.D8,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(7));

            Subscribe(mod | KeyModifiers.LShift, Keys.D9,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(8));

            Subscribe(mod, Keys.O,
                () => _context.Windows.DumpWindowDebugOutput());

            Subscribe(mod | KeyModifiers.LShift, Keys.O,
                () => _context.Windows.DumpWindowUnderCursorDebugOutput());

            Subscribe(mod | KeyModifiers.LShift, Keys.I,
                () => _context.ToggleConsoleWindow());
        }
    }
}
