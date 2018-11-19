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

        private PipeServer _pipeServer;
        private ConfigContext _context;
        private Timer _timer;

        private KeybindManager _keybinds;
        private WorkspaceManager _workspaces;
        private PluginManager _plugins;
        private SystemTrayManager _systemTray;
        private WindowsManager _windows;

        public Workspacer()
        {
            _pipeServer = new PipeServer();
            _timer = new Timer();
            _timer.Elapsed += (s, e) => UpdateActiveHandles();
            _timer.Interval = 5000;
        }

        public void Start()
        {
            ConsoleHelper.Initialize();
            Logger.Initialize(Console.Out);
            Logger.Debug("starting Workspacer");

            _pipeServer.Start();
            _timer.Enabled = true;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            _context = new ConfigContext(_pipeServer);

            _context.Plugins = _plugins = new PluginManager();
            _context.SystemTray = _systemTray = new SystemTrayManager();
            _context.Workspaces = _workspaces = new WorkspaceManager(_context);
            _context.Windows = _windows = new WindowsManager();
            _context.Keybinds = _keybinds = new KeybindManager(_context);

            _windows.WindowCreated += _workspaces.AddWindow;
            _windows.WindowDestroyed += _workspaces.RemoveWindow;
            _windows.WindowUpdated += _workspaces.UpdateWindow;

            _systemTray.AddToContextMenu("Toggle Enabled/Disabled", () => _context.Enabled = !_context.Enabled);
            _systemTray.AddToContextMenu("Quit Workspacer", () => _context.Quit());
            _systemTray.AddToContextMenu("Restart Workspacer", () => _context.Restart());
            if (ConfigHelper.CanCreateExampleConfig())
            {
                _systemTray.AddToContextMenu("Create example Workspacer.config.csx", CreateExampleConfig);
            }

            _workspaces.InitializeMonitors();

            ConfigHelper.DoConfig(_context);

            _windows.Initialize();

            var allWorkspaces = _context.WorkspaceContainer.GetAllWorkspaces().ToList();
            // check to make sure there are enough workspaces for the monitors
            if (_workspaces.Monitors.Count() > allWorkspaces.Count)
            {
                throw new Exception("you must specify at least enough workspaces to cover all monitors");
            }

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

            foreach (var workspace in _context.WorkspaceContainer.GetAllWorkspaces())
            {
                workspace.DoLayout();
            }

            _plugins.AfterConfig(_context);

            FocusStealer.Initialize();
            while(true) { }
        }

        private void SendResponse(LauncherResponse response)
        {
            var str = JsonConvert.SerializeObject(response);
            _pipeServer.SendResponse(str);
        }

        public void Quit()
        {
            _context.Quit();
        }

        private void UpdateActiveHandles()
        {
            var response = new LauncherResponse()
            {
                Action = LauncherAction.UpdateHandles,
                ActiveHandles = _workspaces.GetActiveHandles().Select(h => h.ToInt64()).ToList(),
            };
            SendResponse(response);
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
