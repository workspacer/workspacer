using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer
{
    
    public partial class KeybindManager : IKeybindManager
    {
        
        private class NamedBind<I>
        {
            public I Binding { get; }
            public string Name { get; }

            public NamedBind(I binding, string name)
            {
                Binding = binding;
                Name = name;
            }
        }

        private Logger Logger = Logger.Create();
        private Win32.HookProc _kbdHook;
        private Win32.HookProc _mouseHook;

        private IConfigContext _context;
        private IDictionary<Sub, NamedBind<KeybindHandler>> _kbdSubs;
        private IDictionary<MouseEvent, NamedBind<MouseHandler>> _mouseSubs;

        private IDictionary<string, IDictionary<Sub, NamedBind<KeybindHandler>>> _kbdmodes;
        private IDictionary<string, IDictionary<MouseEvent, NamedBind<MouseHandler>>> _mousemodes;
        private TextBlockMessage _keybindDialog;
        private TextBlockMessage _keybindWarning;
        private string _currentmode;

        public KeybindManager(IConfigContext context)
        {
            _context = context;
            _kbdHook = KbdHook;
            _mouseHook = MouseHook;
            _kbdSubs = new Dictionary<Sub, NamedBind<KeybindHandler>>();
            _mouseSubs = new Dictionary<MouseEvent, NamedBind<MouseHandler>>();
            
            _kbdmodes = new Dictionary<string, IDictionary<Sub, NamedBind<KeybindHandler>>> ();
            _mousemodes = new Dictionary<string, IDictionary<MouseEvent, NamedBind<MouseHandler>>>();
            _currentmode = "default";
            
            CreateMode("default", ModeDefaultBindings.All);
            SetMode("default");
            
        
            var thread = new Thread(() =>
            {
                Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, _kbdHook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
                Win32.SetWindowsHookEx(Win32.WH_MOUSE_LL, _mouseHook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
                Application.Run();
            });
            thread.Name = "KeybindManager";
            thread.Start();
        }

        
        public void CreateMode(string name,ModeDefaultBindings defaultBindings)
        {
            _kbdmodes.Add(name, new Dictionary<Sub, NamedBind<KeybindHandler>>());
            _mousemodes.Add(name, new Dictionary<MouseEvent, NamedBind<MouseHandler>>());
            SubscribeDefaults(name, defaultBindings);
        }

        public void SetMode(string name)
        {
            _kbdSubs = _kbdmodes[name];
            _mouseSubs = _mousemodes[name];
            _currentmode = name;
            Debug.WriteLine(name);
        }

        public string GetCurrentMode()
        {
            return _currentmode;
        }
       
      

        public void ShowKeybindWarning(string warningMessage)
        {
            _keybindWarning = new TextBlockMessage("workspacer keybinds", "Warning, duplicate keybinds!", warningMessage, new List<Tuple<string, Action>>()
            {
                new Tuple<string, Action>("ok", () => { }),
            });
          
            _keybindWarning.Show();
            
        }

        public void Subscribe(string mode, KeyModifiers mod, Keys key, KeybindHandler handler, string name)
        {
            var sub = new Sub(mod, key);

       
            if (_kbdmodes[mode].ContainsKey(sub))
            {
                Logger.Error($" The Key Combination `{mod}-{key}` is already bound to action: `{name}`");
                string warning = @$" The Key Combination `{mod}-{key}` is already bound to action: `{name}`. To fix this go into your config file and compare assignments of hotkeys
You can either change your custom hotkey or reassign the default hotkey";

                ShowKeybindWarning(warning);
            }
            _kbdmodes[mode][sub] = new NamedBind<KeybindHandler>(handler, name);
        
        }

        public void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler, string name)
        {
            var sub = new Sub(mod, key);
            if (_kbdSubs.ContainsKey(sub))
            {
               Logger.Error($" The Key Combination `{mod}-{key}` is already bound to action: `{name}`");
               string warning = @$" The Key Combination `{mod}-{key}` is already bound to action: `{name}`. To fix this go into your config file and compare assignments of hotkeys
You can either change your custom hotkey or reassign the default hotkey";

                ShowKeybindWarning(warning);
            }
            _kbdSubs[sub] = new NamedBind<KeybindHandler>(handler, name);
        }

        public void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler)
        {
            Subscribe(mod, key, handler, null);
        }

        public void Subscribe(string mode,MouseEvent evt, MouseHandler handler, string name)
        {
            _mouseSubs[evt] = new NamedBind<MouseHandler>(handler, name);
        }

        public void Subscribe(string mode,MouseEvent evt, MouseHandler handler)
        {
            Subscribe(mode, evt, handler, null);
        }

        public void Unsubscribe(KeyModifiers mod, Keys key)
        {
            var sub = new Sub(mod, key);
            if (_kbdSubs.ContainsKey(sub))
            {
                _kbdSubs.Remove(sub);
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

        public void SubscribeDefaults(string modeName, ModeDefaultBindings defaultBindings, KeyModifiers mod = KeyModifiers.LAlt)
        {
            if (defaultBindings != ModeDefaultBindings.None)
            {
                Subscribe(modeName, MouseEvent.LButtonDown,
                    () => _context.Workspaces.SwitchFocusedMonitorToMouseLocation());

                Subscribe(modeName, mod, Keys.Escape,
                    () => _context.Enabled = !_context.Enabled, "toggle enable/disable");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.C,
                    () => _context.Workspaces.FocusedWorkspace.CloseFocusedWindow(), "close focused window");

                Subscribe(modeName, mod, Keys.Space,
                    () => _context.Workspaces.FocusedWorkspace.NextLayoutEngine(), "next layout");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.Space,
                    () => _context.Workspaces.FocusedWorkspace.PreviousLayoutEngine(), "previous layout");

                Subscribe(modeName, mod, Keys.N,
                    () => _context.Workspaces.FocusedWorkspace.ResetLayout(), "reset layout");

                Subscribe(modeName, mod, Keys.J,
                    () => _context.Workspaces.FocusedWorkspace.FocusNextWindow(), "focus next window");

                Subscribe(modeName, mod, Keys.K,
                    () => _context.Workspaces.FocusedWorkspace.FocusPreviousWindow(), "focus previous window");

                Subscribe(modeName, mod, Keys.M,
                    () => _context.Workspaces.FocusedWorkspace.FocusPrimaryWindow(), "focus primary window");

                Subscribe(modeName, mod, Keys.Enter,
                    () => _context.Workspaces.FocusedWorkspace.SwapFocusAndPrimaryWindow(), "swap focus and primary window");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.J,
                    () => _context.Workspaces.FocusedWorkspace.SwapFocusAndNextWindow(), "swap focus and next window");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.K,
                    () => _context.Workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow(), "swap focus and previous window");

                Subscribe(modeName, mod, Keys.H,
                    () => _context.Workspaces.FocusedWorkspace.ShrinkPrimaryArea(), "shrink primary area");

                Subscribe(modeName, mod, Keys.L,
                    () => _context.Workspaces.FocusedWorkspace.ExpandPrimaryArea(), "expand primary area");

                Subscribe(modeName, mod, Keys.Oemcomma,
                    () => _context.Workspaces.FocusedWorkspace.IncrementNumberOfPrimaryWindows(), "increment # primary windows");

                Subscribe(modeName, mod, Keys.OemPeriod,
                    () => _context.Workspaces.FocusedWorkspace.DecrementNumberOfPrimaryWindows(), "decrement # primary windows");

                Subscribe(modeName, mod, Keys.T,
                    () => _context.Windows.ToggleFocusedWindowTiling(), "toggle tiling for focused window");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.Q, _context.Quit, "quit workspacer");

                Subscribe(modeName, mod, Keys.Q, _context.Restart, "restart workspacer");

                Subscribe(modeName, mod, Keys.D1,
                    () => _context.Workspaces.SwitchToWorkspace(0), "switch to workspace 1");

                Subscribe(modeName, mod, Keys.D2,
                    () => _context.Workspaces.SwitchToWorkspace(1), "switch to workspace 2");

                Subscribe(modeName, mod, Keys.D3,
                    () => _context.Workspaces.SwitchToWorkspace(2), "switch to workspace 3");

                Subscribe(modeName, mod, Keys.D4,
                    () => _context.Workspaces.SwitchToWorkspace(3), "switch to workspace 4");

                Subscribe(modeName, mod, Keys.D5,
                    () => _context.Workspaces.SwitchToWorkspace(4), "switch to workspace 5");

                Subscribe(modeName, mod, Keys.D6,
                    () => _context.Workspaces.SwitchToWorkspace(5), "switch to workspace 6");

                Subscribe(modeName, mod, Keys.D7,
                    () => _context.Workspaces.SwitchToWorkspace(6), "switch to workspace 7");

                Subscribe(modeName, mod, Keys.D8,
                    () => _context.Workspaces.SwitchToWorkspace(7), "switch to workspace 8");

                Subscribe(modeName, mod, Keys.D9,
                    () => _context.Workspaces.SwitchToWorkspace(8), "switch to workpsace 9");

                Subscribe(modeName, mod, Keys.Left,
                    () => _context.Workspaces.SwitchToPreviousWorkspace(), "switch to previous workspace");

                Subscribe(modeName, mod, Keys.Right,
                    () => _context.Workspaces.SwitchToNextWorkspace(), "switch to next workspace");
            }
            if (defaultBindings != ModeDefaultBindings.Minimal)
            {
                Subscribe(modeName, mod, Keys.Oemtilde,
                    () => _context.Workspaces.SwitchToLastFocusedWorkspace(), "switch to last focused workspace");

                Subscribe(modeName, mod, Keys.W,
                    () => _context.Workspaces.SwitchFocusedMonitor(0), "focus monitor 1");

                Subscribe(modeName, mod, Keys.E,
                    () => _context.Workspaces.SwitchFocusedMonitor(1), "focus monitor 2");

                Subscribe(modeName, mod, Keys.R,
                    () => _context.Workspaces.SwitchFocusedMonitor(2), "focus monitor 3");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.W,
                    () => _context.Workspaces.MoveFocusedWindowToMonitor(0), "move focused window to monitor 1");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.E,
                    () => _context.Workspaces.MoveFocusedWindowToMonitor(1), "move focused window to monitor 2");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.R,
                    () => _context.Workspaces.MoveFocusedWindowToMonitor(2), "move focused window to monitor 3");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D1,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(0), "switch focused window to workspace 1");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D2,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(1), "switch focused window to workspace 2");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D3,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(2), "switch focused window to workspace 3");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D4,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(3), "switch focused window to workspace 4");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D5,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(4), "switch focused window to workspace 5");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D6,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(5), "switch focused window to workspace 6");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D7,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(6), "switch focused window to workspace 7");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D8,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(7), "switch focused window to workspace 8");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.D9,
                    () => _context.Workspaces.MoveFocusedWindowToWorkspace(8), "switch focused window to workspace 9");

                Subscribe(modeName, mod, Keys.O,
                    () => _context.Windows.DumpWindowDebugOutput(), "dump debug info to console for all windows");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.O,
                    () => _context.Windows.DumpWindowUnderCursorDebugOutput(), "dump debug info to console for window under cursor");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.I,
                    () => _context.ToggleConsoleWindow(), "toggle debug console");

                Subscribe(modeName, mod | KeyModifiers.LShift, Keys.Oem2,
                    () => ShowKeybindDialog(), "open keybind window");
            }
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
