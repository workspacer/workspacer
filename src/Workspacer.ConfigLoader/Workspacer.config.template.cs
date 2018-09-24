using System;
using System.Linq;
using Workspacer.Shared;
using Workspacer.ConfigLoader;
using Workspacer.Bar;
using Workspacer.Bar.Widgets;

namespace Workspacer.Config
{
  
	public class Config : IConfig
    {
        public void Configure(IConfigContext context)
        {
            var mod = KeyModifiers.LAlt;
            var barHeight = 30;
            var fontSize = 16;
            var defaultForeground = Color.White;
            var defaultBackground = Color.Black;

            context.Plugins.RegisterPlugin(new BarPlugin(new BarPluginConfig()
            {
                BarHeight = barHeight,
                FontSize = fontSize,
                DefaultWidgetForeground = defaultForeground,
                DefaultWidgetBackground = defaultBackground,
                LeftWidgets = () => new IBarWidget[] { new WorkspaceWidget(), new TextWidget(": "), new TitleWidget() },
                RightWidgets = () => new IBarWidget[] { new TimeWidget(), new ActiveLayoutWidget() },
            }));

            context.Layouts.AddLayout(() => new MenuBarLayoutEngine(new TallLayoutEngine(1, 0.5, 0.03), "Workspacer.Bar", barHeight));
            context.Layouts.AddLayout(() => new MenuBarLayoutEngine(new FullLayoutEngine(), "Workspacer.Bar", barHeight));
            context.Layouts.AddLayout(() => new MenuBarLayoutEngine(new VertLayoutEngine(), "Workspacer.Bar", barHeight));
            context.Layouts.AddLayout(() => new MenuBarLayoutEngine(new HorzLayoutEngine(), "Workspacer.Bar", barHeight));

            context.Workspaces.WorkspaceSelectorFunc = (window) =>
            {
                if (context.Keybinds.KeyIsPressed(Keys.LMenu))
                {
                    return context.Workspaces.FocusedWorkspace;
                }

                return context.Workspaces.FocusedWorkspace;
            };

            context.Workspaces.WindowFilterFunc = (window) => 
            {
                if (window.Title.Contains("Workspacer.Bar"))
                    return false;
                if (window.Title.Contains("Task Manager"))
                    return false;

                return true;
            };

            context.Workspaces.AddWorkspace("one", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("two", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("three", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("four", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("five", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("six", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("seven", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("eight", context.Layouts.CreateLayouts());
            context.Workspaces.AddWorkspace("nine", context.Layouts.CreateLayouts());

            context.Keybinds.Subscribe(MouseEvent.LButtonDown,
                () => context.Workspaces.SwitchFocusedMonitorToMouseLocation());

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.E,
                () => context.Enabled = !context.Enabled);

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.C,
                () => context.Workspaces.FocusedWorkspace.CloseFocusedWindow());

            context.Keybinds.Subscribe(mod, Keys.Space,
                () => context.Workspaces.FocusedWorkspace.NextLayoutEngine());

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.Space,
                () => context.Workspaces.FocusedWorkspace.PreviousLayoutEngine());

            context.Keybinds.Subscribe(mod, Keys.N,
                () => context.Workspaces.FocusedWorkspace.ResetLayout());

            context.Keybinds.Subscribe(mod, Keys.J,
                () => context.Workspaces.FocusedWorkspace.FocusNextWindow());

            context.Keybinds.Subscribe(mod, Keys.K,
                () => context.Workspaces.FocusedWorkspace.FocusPreviousWindow());

            context.Keybinds.Subscribe(mod, Keys.M,
                () => context.Workspaces.FocusedWorkspace.FocusMasterWindow());

            context.Keybinds.Subscribe(mod, Keys.Enter,
                () => context.Workspaces.FocusedWorkspace.SwapFocusAndMasterWindow());

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.J,
                () => context.Workspaces.FocusedWorkspace.SwapFocusAndNextWindow());

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.K,
                () => context.Workspaces.FocusedWorkspace.SwapFocusAndPreviousWindow());

            context.Keybinds.Subscribe(mod, Keys.H,
                () => context.Workspaces.FocusedWorkspace.ShrinkMasterArea());

            context.Keybinds.Subscribe(mod, Keys.L,
                () => context.Workspaces.FocusedWorkspace.ExpandMasterArea());

            context.Keybinds.Subscribe(mod, Keys.Oemcomma,
                () => context.Workspaces.FocusedWorkspace.IncrementNumberOfMasterWindows());

            context.Keybinds.Subscribe(mod, Keys.OemPeriod,
                () => context.Workspaces.FocusedWorkspace.DecrementNumberOfMasterWindows());

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.Q, context.Quit);

            context.Keybinds.Subscribe(mod, Keys.Q, context.Restart);

            context.Keybinds.Subscribe(mod, Keys.D1,
                () => context.Workspaces.SwitchToWorkspace(0));

            context.Keybinds.Subscribe(mod, Keys.D2,
                () => context.Workspaces.SwitchToWorkspace(1));

            context.Keybinds.Subscribe(mod, Keys.D3,
                () => context.Workspaces.SwitchToWorkspace(2));

            context.Keybinds.Subscribe(mod, Keys.D4,
                () => context.Workspaces.SwitchToWorkspace(3));

            context.Keybinds.Subscribe(mod, Keys.D5,
                () => context.Workspaces.SwitchToWorkspace(4));

            context.Keybinds.Subscribe(mod, Keys.D6,
                () => context.Workspaces.SwitchToWorkspace(5));

            context.Keybinds.Subscribe(mod, Keys.D7,
                () => context.Workspaces.SwitchToWorkspace(6));

            context.Keybinds.Subscribe(mod, Keys.D8,
                () => context.Workspaces.SwitchToWorkspace(7));

            context.Keybinds.Subscribe(mod, Keys.D9,
                () => context.Workspaces.SwitchToWorkspace(8));

            context.Keybinds.Subscribe(mod, Keys.W,
                () => context.Workspaces.SwitchFocusedMonitor(0));

            context.Keybinds.Subscribe(mod, Keys.E,
                () => context.Workspaces.SwitchFocusedMonitor(1));

            context.Keybinds.Subscribe(mod, Keys.R,
                () => context.Workspaces.SwitchFocusedMonitor(2));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.W,
                () => context.Workspaces.MoveFocusedWindowToMonitor(0));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.E,
                () => context.Workspaces.MoveFocusedWindowToMonitor(1));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.R,
                () => context.Workspaces.MoveFocusedWindowToMonitor(2));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D1,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(0));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D2,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(1));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D3,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(2));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D4,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(3));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D5,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(4));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D6,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(5));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D7,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(6));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D8,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(7));

            context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.D9,
                () => context.Workspaces.MoveFocusedWindowToWorkspace(8));
        }
    }
}
