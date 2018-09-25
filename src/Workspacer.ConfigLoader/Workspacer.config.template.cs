using System;
using System.Linq;
using Workspacer.Shared;
using Workspacer.ConfigLoader;
using Workspacer.Bar;
using Workspacer.Bar.Widgets;
using System.Diagnostics;

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
            var barTitle = "Workspacer.Bar";

            context.Plugins.RegisterPlugin(new BarPlugin(new BarPluginConfig()
            {
                BarHeight = barHeight,
                FontSize = fontSize,
                DefaultWidgetForeground = defaultForeground,
                DefaultWidgetBackground = defaultBackground,
                LeftWidgets = () => new IBarWidget[] { new WorkspaceWidget(), new TextWidget(": "), new TitleWidget() },
                RightWidgets = () => new IBarWidget[] { new TimeWidget(), new ActiveLayoutWidget() },
            }));

            Func<ILayoutEngine, ILayoutEngine> wrapLayout = (ILayoutEngine inner) => new MenuBarLayoutEngine(inner, barTitle, barHeight);
            Func<ILayoutEngine[]> createLayouts = () => new ILayoutEngine[]
            {
                wrapLayout(new TallLayoutEngine(1, 0.5, 0.03)),
                wrapLayout(new FullLayoutEngine()),
                wrapLayout(new VertLayoutEngine()),
                wrapLayout(new HorzLayoutEngine()),
            };

            context.Workspaces.WindowFilterFunc = (window) => 
            {
                if (window.Title.Contains("Task Manager"))
                    return false;
                if (window.Process.Id == Process.GetCurrentProcess().Id)
                    return false;

                return true;
            };

            context.Workspaces.AddWorkspace("one", createLayouts());
            context.Workspaces.AddWorkspace("two", createLayouts());
            context.Workspaces.AddWorkspace("three", createLayouts());
            context.Workspaces.AddWorkspace("four", createLayouts());
            context.Workspaces.AddWorkspace("five", createLayouts());


            context.Keybinds.SubscribeDefaults(context, mod);
        }
    }
}
