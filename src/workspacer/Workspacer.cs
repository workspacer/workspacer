using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace workspacer
{
    public class workspacer
    {
        private static Logger Logger = Logger.Create();
        public static bool Enabled { get; set; }

        private ConfigContext _context;

        public void Start()
        {
            // init user folder
            FileHelper.EnsureConfigDirectoryExists();

            // init logging
            Logger.Initialize(FileHelper.GetConfigDirectory());
            Logger.Debug("starting workspacer");

            // init plugin assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            // init context and managers
            _context = new ConfigContext();

            // connect to watcher
            _context.ConnectToWatcher();

            // attach console output target
            Logger.AttachConsoleLogger((str) =>
            {
                Console.WriteLine(str);
                _context.SendLogToConsole(str);
            });

            // init system tray
            _context.SystemTray.AddToContextMenu("Show/Hide keybindings help", () => _context.Keybinds.ShowKeybindDialog());
            _context.SystemTray.AddToContextMenu("Enable/Disable workspacer", () => _context.Enabled = !_context.Enabled);
            _context.SystemTray.AddToContextMenu("Restart workspacer", () => _context.Restart());
            _context.SystemTray.AddToContextMenu("Quit workspacer", () => _context.Quit());
            if (ConfigHelper.CanCreateExampleConfig())
            {
                _context.SystemTray.AddToContextMenu("create example workspacer.config.csx", CreateExampleConfig);
            }

            // init config
            ConfigHelper.DoConfig(_context);

            // init windows
            _context.Windows.Initialize();

            // verify config
            var allWorkspaces = _context.WorkspaceContainer.GetAllWorkspaces().ToList();
            // check to make sure there are enough workspaces for the monitors
            if (_context.MonitorContainer.NumMonitors > allWorkspaces.Count)
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

            // start message pump on main thread
            Application.Run();
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
            if (_context == null)
            {
                return null;
            }

            var match = _context.Plugins.AvailablePlugins.Select(p => p.Assembly).SingleOrDefault(a => a.GetName().FullName == args.Name);
            if (match != null)
            {
                // LoadFrom is used because it loads non-project plugin dependencies
                // https://docs.microsoft.com/en-us/archive/blogs/suzcook/loadfile-vs-loadfrom
                return Assembly.LoadFrom(match.Location);
            }
            return null;
        }

        private void CreateExampleConfig()
        {
            if (!ConfigHelper.CanCreateExampleConfig())
            {
                DisplayMessage("workspacer.config.csx already exists, so one cannot be created.");
            }
            else
            {
                ConfigHelper.CreateExampleConfig();
                DisplayMessage($"workspacer.config.csx created in: [${FileHelper.GetConfigDirectory()}]");
            }
        }

        public void DisplayMessage(string message)
        {
            MessageBox.Show(message, "workspacer");
        }
    }
}
