using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    /// <summary>
    /// provides interfaces and methods that allow interaction and configuration of workspacer
    /// </summary>
    /// <remarks>
    /// IConfigContext is the center point around which user configuration is written. most
    /// actions and behaviors that are user customizable are accessed through the config context
    /// </remarks>
    public interface IConfigContext
    {
        IKeybindManager Keybinds { get; }
        IPluginManager Plugins { get; }
        IWorkspaceManager Workspaces { get; }
        ISystemTrayManager SystemTray { get; }
        IKeyMode KeyMode { get; set; }
        IWindowsManager Windows { get; }
        IWorkspaceContainer WorkspaceContainer { get; set; }
        IWindowRouter WindowRouter { get; set; }

        IMonitorContainer MonitorContainer { get; set; }

        /// <summary>
        /// the default layout Func is used to generate layouts for workspaces that do not
        /// specify custom layouts when created
        /// </summary>
        /// <value>a function that returns an array of new instances of the preferred default layouts</value>
        Func<ILayoutEngine[]> DefaultLayouts { get; set; }

       
        /// <summary>
        /// adds a layout proxy to the context.
        /// layout proxies are used to provide additional functionality to all layout engines across all workspaces
        /// </summary>
        /// <param name="proxy">function that proxies a layout engine</param>
        void AddLayoutProxy(Func<ILayoutEngine, ILayoutEngine> proxy);

        /// <summary>
        /// proxies a set of layouts using the configured proxies
        /// </summary>
        /// <param name="layouts">set of layouts to be proxied</param>
        /// <returns>a set of proxied layouts</returns>
        IEnumerable<ILayoutEngine> ProxyLayouts(IEnumerable<ILayoutEngine> layouts);

        /// <summary>
        /// ConsoleLogLevel controls the log level of the debug output window (alt-shift-i by default)
        /// </summary>
        /// <value>the desired console log level</value>
        LogLevel ConsoleLogLevel { get; set; }

        /// <summary>
        /// FileLogLevel controls the log level for workspacer.log
        /// </summary>
        /// <value>the desired file log level</value>
        LogLevel FileLogLevel { get; set; }

        /// <summary>
        /// toggles visibility of the debug output window
        /// </summary>
        void ToggleConsoleWindow();

        /// <summary>
        /// whether workspacer is enabled or disabled. can be set to <code>false</code> to disable workspacer
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// quit workspacer
        /// </summary>
        void Quit();

        /// <summary>
        /// quit workspacer, specifying an exception that occurred that neccesitates the exit
        /// </summary>
        /// <param name="e"></param>
        void QuitWithException(Exception e);

        /// <summary>
        /// restart workspacer
        /// </summary>
        void Restart();
    }
}
