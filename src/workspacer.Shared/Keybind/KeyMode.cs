using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{ 
    public class NamedBind<I>
     {
        public I Binding { get; }
        public string Name { get; }

        public NamedBind(I binding, string name)
        {
            Binding = binding;
            Name = name;
        }
     }

    public class KeyMode : IKeyMode
    {
     
        public IDictionary<Sub, NamedBind<KeybindHandler>> KeyboardBindings { get; }
        public IDictionary<MouseEvent, NamedBind<MouseHandler>> MouseBindings { get; }
        public string Name { get; }


        private Logger Logger = Logger.Create();
        private IConfigContext _context;

        public KeyMode() { }
        public KeyMode(IConfigContext context, string name)
        {
            
            Name = name;
            KeyboardBindings = new Dictionary<Sub, NamedBind<KeybindHandler>>();
            MouseBindings = new Dictionary<MouseEvent, NamedBind<MouseHandler>>();
            _context = context;
        }

        public void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler, string name)
        {
            var sub = new Sub(mod, key);
            if (KeyboardBindings.ContainsKey(sub))
            {
                Logger.Error($" The Key Combination `{mod}-{key}` is already bound to action: `{name}`");
                string warning = @$" The Key Combination `{mod}-{key}` is already bound to action: `{name}`. To fix this go into your config file and compare assignments of hotkeys
You can either change your custom hotkey or reassign the default hotkey";
            }
            KeyboardBindings[sub] = new NamedBind<KeybindHandler>(handler, name);
        }

        public void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler)
        {
            Subscribe(mod, key, handler, null);
        }

        public void Subscribe(MouseEvent evt, MouseHandler handler, string name)
        {
            MouseBindings[evt] = new NamedBind<MouseHandler>(handler, name);
        }

        public void Subscribe(MouseEvent evt, MouseHandler handler)
        {
            Subscribe(evt, handler, null);
        }

        public void Unsubscribe(KeyModifiers mod, Keys key)
        {
            var sub = new Sub(mod, key);
            if (KeyboardBindings.ContainsKey(sub))
            {
                KeyboardBindings.Remove(sub);
            }
        }

        public void Unsubscribe(MouseEvent evt)
        {
            if (MouseBindings.ContainsKey(evt))
                MouseBindings.Remove(evt);
        }

        public void UnsubscribeAll()
        {
            KeyboardBindings.Clear();
            MouseBindings.Clear();
        }

        public void SubscribeDefaults(KeyModifiers mod = KeyModifiers.LAlt)
        {

            Subscribe(MouseEvent.LButtonDown,
                () => _context.Workspaces.SwitchFocusedMonitorToMouseLocation());

            Subscribe(mod, Keys.Escape,
                () => _context.Enabled = !_context.Enabled, "toggle enable/disable");

            Subscribe(mod | KeyModifiers.LShift, Keys.C,
                () => _context.Workspaces.FocusedWorkspace.CloseFocusedWindow(), "close focused window");

            Subscribe(mod, Keys.Space,
                () => _context.Workspaces.FocusedWorkspace.NextLayoutEngine(), "next layout");

            Subscribe(mod | KeyModifiers.LShift, Keys.Space,
                () => _context.Workspaces.FocusedWorkspace.PreviousLayoutEngine(), "previous layout");

            Subscribe(mod, Keys.N,
                () => _context.Workspaces.FocusedWorkspace.ResetLayout(), "reset layout");

            Subscribe(mod, Keys.J,
                () => _context.Workspaces.FocusedWorkspace.FocusNextWindow(), "focus next window");

            Subscribe(mod, Keys.K,
                () => _context.Workspaces.FocusedWorkspace.FocusPreviousWindow(), "focus previous window");

            Subscribe(mod, Keys.M,
                () => _context.Workspaces.FocusedWorkspace.FocusPrimaryWindow(), "focus primary window");

            Subscribe(mod, Keys.Enter,
                () => _context.Workspaces.FocusedWorkspace.SwapFocusAndPrimaryWindow(), "swap focus and primary window");

            Subscribe(mod | KeyModifiers.LShift, Keys.J,
                () => _context.Workspaces.FocusedWorkspace.SwapFocusAndNextWindow(), "swap focus and next window");

            Subscribe(mod | KeyModifiers.LShift, Keys.K,
                () => _context.Workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow(), "swap focus and previous window");

            Subscribe(mod, Keys.H,
                () => _context.Workspaces.FocusedWorkspace.ShrinkPrimaryArea(), "shrink primary area");

            Subscribe(mod, Keys.L,
                () => _context.Workspaces.FocusedWorkspace.ExpandPrimaryArea(), "expand primary area");

            Subscribe(mod, Keys.Oemcomma,
                () => _context.Workspaces.FocusedWorkspace.IncrementNumberOfPrimaryWindows(), "increment # primary windows");

            Subscribe(mod, Keys.OemPeriod,
                () => _context.Workspaces.FocusedWorkspace.DecrementNumberOfPrimaryWindows(), "decrement # primary windows");

            Subscribe(mod, Keys.T,
                () => _context.Windows.ToggleFocusedWindowTiling(), "toggle tiling for focused window");

            Subscribe(mod | KeyModifiers.LShift, Keys.Q, _context.Quit, "quit workspacer");

            Subscribe(mod, Keys.Q, _context.Restart, "restart workspacer");

            Subscribe(mod, Keys.D1,
                () => _context.Workspaces.SwitchToWorkspace(0), "switch to workspace 1");

            Subscribe(mod, Keys.D2,
                () => _context.Workspaces.SwitchToWorkspace(1), "switch to workspace 2");

            Subscribe(mod, Keys.D3,
                () => _context.Workspaces.SwitchToWorkspace(2), "switch to workspace 3");

            Subscribe(mod, Keys.D4,
                () => _context.Workspaces.SwitchToWorkspace(3), "switch to workspace 4");

            Subscribe(mod, Keys.D5,
                () => _context.Workspaces.SwitchToWorkspace(4), "switch to workspace 5");

            Subscribe(mod, Keys.D6,
                () => _context.Workspaces.SwitchToWorkspace(5), "switch to workspace 6");

            Subscribe(mod, Keys.D7,
                () => _context.Workspaces.SwitchToWorkspace(6), "switch to workspace 7");

            Subscribe(mod, Keys.D8,
                () => _context.Workspaces.SwitchToWorkspace(7), "switch to workspace 8");

            Subscribe(mod, Keys.D9,
                () => _context.Workspaces.SwitchToWorkspace(8), "switch to workpsace 9");

            Subscribe(mod, Keys.Left,
                () => _context.Workspaces.SwitchToPreviousWorkspace(), "switch to previous workspace");

            Subscribe(mod, Keys.Right,
                () => _context.Workspaces.SwitchToNextWorkspace(), "switch to next workspace");

            Subscribe(mod, Keys.Oemtilde,
                () => _context.Workspaces.SwitchToLastFocusedWorkspace(), "switch to last focused workspace");

            Subscribe(mod, Keys.W,
                () => _context.Workspaces.SwitchFocusedMonitor(0), "focus monitor 1");

            Subscribe(mod, Keys.E,
                () => _context.Workspaces.SwitchFocusedMonitor(1), "focus monitor 2");

            Subscribe(mod, Keys.R,
                () => _context.Workspaces.SwitchFocusedMonitor(2), "focus monitor 3");

            Subscribe(mod | KeyModifiers.LShift, Keys.W,
                () => _context.Workspaces.MoveFocusedWindowToMonitor(0), "move focused window to monitor 1");

            Subscribe(mod | KeyModifiers.LShift, Keys.E,
                () => _context.Workspaces.MoveFocusedWindowToMonitor(1), "move focused window to monitor 2");

            Subscribe(mod | KeyModifiers.LShift, Keys.R,
                () => _context.Workspaces.MoveFocusedWindowToMonitor(2), "move focused window to monitor 3");

            Subscribe(mod | KeyModifiers.LShift, Keys.D1,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(0), "switch focused window to workspace 1");

            Subscribe(mod | KeyModifiers.LShift, Keys.D2,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(1), "switch focused window to workspace 2");

            Subscribe(mod | KeyModifiers.LShift, Keys.D3,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(2), "switch focused window to workspace 3");

            Subscribe(mod | KeyModifiers.LShift, Keys.D4,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(3), "switch focused window to workspace 4");

            Subscribe(mod | KeyModifiers.LShift, Keys.D5,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(4), "switch focused window to workspace 5");

            Subscribe(mod | KeyModifiers.LShift, Keys.D6,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(5), "switch focused window to workspace 6");

            Subscribe(mod | KeyModifiers.LShift, Keys.D7,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(6), "switch focused window to workspace 7");

            Subscribe(mod | KeyModifiers.LShift, Keys.D8,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(7), "switch focused window to workspace 8");

            Subscribe(mod | KeyModifiers.LShift, Keys.D9,
                () => _context.Workspaces.MoveFocusedWindowToWorkspace(8), "switch focused window to workspace 9");

            Subscribe(mod, Keys.O,
                () => _context.Windows.DumpWindowDebugOutput(), "dump debug info to console for all windows");

            Subscribe(mod | KeyModifiers.LShift, Keys.O,
                () => _context.Windows.DumpWindowUnderCursorDebugOutput(), "dump debug info to console for window under cursor");

            Subscribe(mod | KeyModifiers.LShift, Keys.I,
                () => _context.ToggleConsoleWindow(), "toggle debug console");
        }
    }
}
