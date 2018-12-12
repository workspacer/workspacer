using System;
using System.Linq;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;
using System.Reflection;
using System.IO;

namespace workspacer
{
    public class workspacer
    {
        private static Logger Logger = Logger.Create();
        public static bool Enabled { get; set; }

        private ConfigContext _context;

        public void Start()
        {
            // init logging
            ConsoleHelper.Initialize();
            Logger.Initialize(Console.Out);
            Logger.Debug("starting workspacer");

            // init plugin assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            // init context and managers
            _context = new ConfigContext();

            // connect to watcher
            _context.ConnectToWatcher();

            // init system tray
            _context.SystemTray.AddToContextMenu("Toggle Enabled/Disabled", () => _context.Enabled = !_context.Enabled);
            _context.SystemTray.AddToContextMenu("Quit workspacer", () => _context.Quit());
            _context.SystemTray.AddToContextMenu("Restart workspacer", () => _context.Restart());
            if (ConfigHelper.CanCreateExampleConfig())
            {
                _context.SystemTray.AddToContextMenu("Create example workspacer.config.csx", CreateExampleConfig);
            }

            // init monitors
            _context.Workspaces.InitializeMonitors();

            // init config
            ConfigHelper.DoConfig(_context);

            // init windows
            _context.Windows.Initialize();

            // verify config
            var allWorkspaces = _context.WorkspaceContainer.GetAllWorkspaces().ToList();
            // check to make sure there are enough workspaces for the monitors
            if (_context.Workspaces.Monitors.Count() > allWorkspaces.Count)
            {
                throw new Exception("you must specify at least enough workspaces to cover all monitors");
            }

            // init workspaces
            var state = _context.LoadState();
            if (state != null)
            {
                _context.Workspaces.InitializeWithState(state.WorkspaceState, _context.Windows.Windows);
                Enabled = true;
            }
            else
            {
                _context.Workspaces.Initialize(_context.Windows.Windows);
                Enabled = true;
                _context.Workspaces.SwitchToWorkspace(0);
            }

            // force first layout
            foreach (var workspace in _context.WorkspaceContainer.GetAllWorkspaces())
            {
                workspace.DoLayout();
            }

            // notify plugins that config is done
            _context.Plugins.AfterConfig(_context);

            // start focus stealer
            FocusStealer.Initialize();
        }

        public void Quit()
        {
            _context.Quit();
        }

        public void QuitWithException(Exception e)
        {
            _context.QuitWithException(e);
        }

        private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var match = _context.Plugins.AvailablePlugins.Select(p => p.Assembly).SingleOrDefault(a => a.GetName().FullName == args.Name);
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
                DisplayMessage("workspacer.config.csx already exists, so one cannot be created.");
            } else
            {
                ConfigHelper.CreateExampleConfig();
                DisplayMessage($"workspacer.config.csx created in: [${ConfigHelper.GetConfigDirPath()}]");
            }
        }

        public void DisplayMessage(string message)
        {
            var title = GetType().Name.Replace("Verb", "");
            MessageHelper.ShowMessage(title, message);
        }
    }
}
