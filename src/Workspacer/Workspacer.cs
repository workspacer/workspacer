using System;
using System.Linq;
using Newtonsoft.Json;
using Workspacer.ConfigLoader;
using Timer = System.Timers.Timer;
using System.Reflection;
using System.IO;

namespace Workspacer
{
    public class Workspacer
    {
        private static Logger Logger = Logger.Create();
        public static bool Enabled { get; set; }

        private ConfigContext _context;

        private KeybindManager _keybinds;
        private WorkspaceManager _workspaces;
        private PluginManager _plugins;
        private SystemTrayManager _systemTray;
        private WindowsManager _windows;

        public void Start()
        {
            // init logging
            ConsoleHelper.Initialize();
            Logger.Initialize(Console.Out);
            Logger.Debug("starting Workspacer");

            // init plugin assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            // init context and managers
            SetupContext();

            // connect to watcher
            _context.ConnectToWatcher();

            // init system tray
            _systemTray.AddToContextMenu("Toggle Enabled/Disabled", () => _context.Enabled = !_context.Enabled);
            _systemTray.AddToContextMenu("Quit Workspacer", () => _context.Quit());
            _systemTray.AddToContextMenu("Restart Workspacer", () => _context.Restart());
            if (ConfigHelper.CanCreateExampleConfig())
            {
                _systemTray.AddToContextMenu("Create example Workspacer.config.csx", CreateExampleConfig);
            }

            // init monitors
            _workspaces.InitializeMonitors();

            // init config
            ConfigHelper.DoConfig(_context);

            // init windows
            _windows.Initialize();

            // verify config
            var allWorkspaces = _context.WorkspaceContainer.GetAllWorkspaces().ToList();
            // check to make sure there are enough workspaces for the monitors
            if (_workspaces.Monitors.Count() > allWorkspaces.Count)
            {
                throw new Exception("you must specify at least enough workspaces to cover all monitors");
            }

            // init workspaces
            var state = _context.LoadState();
            if (state != null)
            {
                _workspaces.InitializeWithState(state.WorkspaceState, _windows.Windows);
                Enabled = true;
            }
            else
            {
                _workspaces.Initialize(_windows.Windows);
                Enabled = true;
                _workspaces.SwitchToWorkspace(0);
            }

            // force first layout
            foreach (var workspace in _context.WorkspaceContainer.GetAllWorkspaces())
            {
                workspace.DoLayout();
            }

            // notify plugins that config is done
            _plugins.AfterConfig(_context);

            // start focus stealer
            FocusStealer.Initialize();
        }

        private void SetupContext()
        {
            _context = new ConfigContext();

            _context.Plugins = _plugins = new PluginManager();
            _context.SystemTray = _systemTray = new SystemTrayManager();
            _context.Workspaces = _workspaces = new WorkspaceManager(_context);
            _context.Windows = _windows = new WindowsManager();
            _context.Keybinds = _keybinds = new KeybindManager(_context);

            _windows.WindowCreated += _workspaces.AddWindow;
            _windows.WindowDestroyed += _workspaces.RemoveWindow;
            _windows.WindowUpdated += _workspaces.UpdateWindow;
        }

        public void Quit()
        {
            _context.Quit();
        }

        private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var match = _plugins.AvailablePlugins.Select(p => p.Assembly).SingleOrDefault(a => a.GetName().FullName == args.Name);
            if (match != null)
            {
                return Assembly.LoadFile(match.Location);
            }
            return null;
        }

        private void CreateExampleConfig()
        {
            if (!ConfigHelper.CanCreateExampleConfig())
            {
                DisplayMessage("Workspacer.config.csx already exists, so one cannot be created.");
            } else
            {
                ConfigHelper.CreateExampleConfig();
                DisplayMessage($"Workspacer.config.csx created in: [${ConfigHelper.GetConfigDirPath()}]");
            }
        }

        public void DisplayMessage(string message)
        {
            var title = GetType().Name.Replace("Verb", "");
            MessageHelper.ShowMessage(title, message);
        }
    }
}
