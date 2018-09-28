using System;
using System.Linq;
using Newtonsoft.Json;
using Workspacer.Shared;
using Workspacer.ConfigLoader;
using Timer = System.Timers.Timer;
using System.Reflection;
using System.IO;

namespace Workspacer
{
    public class Workspacer
    {
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
            _pipeServer.Start();
            _timer.Enabled = true;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            _keybinds = new KeybindManager();
            _plugins = new PluginManager();
            _systemTray = new SystemTrayManager();
            _workspaces = new WorkspaceManager();
            _windows = new WindowsManager();

            _windows.WindowCreated += _workspaces.AddWindow;
            _windows.WindowDestroyed += _workspaces.RemoveWindow;
            _windows.WindowUpdated += _workspaces.UpdateWindow;

            var stateManager = new StateManager(_workspaces);

            _context = new ConfigContext(_pipeServer, stateManager)
            {
                Keybinds = _keybinds,
                Workspaces = _workspaces,
                Plugins = _plugins,
                SystemTray = _systemTray,
                Windows = _windows,
            };


            _systemTray.AddToContextMenu("Toggle Enabled/Disabled", () => _context.Enabled = !_context.Enabled);
            _systemTray.AddToContextMenu("Quit Workspacer", () => _context.Quit());
            _systemTray.AddToContextMenu("Restart Workspacer", () => _context.Restart());
            if (CanCreateExampleConfig())
            {
                _systemTray.AddToContextMenu("Create example Workspacer.config.cs", CreateExampleConfig);
            }

            _workspaces.InitializeMonitors();

            var config = GetConfig();
            config.Configure(_context);

            var state = stateManager.LoadState();

            _windows.Initialize();

            var allWorkspaces = _workspaces.Container.GetAllWorkspaces().ToList();
            // check to make sure there are enough workspaces for the monitors
            if (_workspaces.Monitors.Count() > allWorkspaces.Count)
            {
                throw new Exception("you must specify at least enough workspaces to cover all monitors");
            }

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

            foreach (var workspace in _workspaces.Container.GetAllWorkspaces())
            {
                workspace.DoLayout();
            }

            _plugins.AfterConfig(_context);

            FocusStealer.Initialize();
            while(true) { }
        }

        private IConfig GetConfig()
        {
            return ConfigHelper.GetConfig(_plugins.AvailablePlugins);
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

        private bool CanCreateExampleConfig()
        {
            return !File.Exists(ConfigHelper.GetConfigPath());
        }

        private void CreateExampleConfig()
        {
            if (File.Exists(ConfigHelper.GetConfigPath()))
            {
                DisplayMessage("Workspacer.config.cs already exists, so one cannot be created.");
            } else
            {
                File.WriteAllText(ConfigHelper.GetConfigPath(), ConfigHelper.GetConfigTemplate());
                DisplayMessage($"Workspacer.config.cs created at: [${ConfigHelper.GetConfigPath()}]");
            }
        }

        public void DisplayMessage(string message)
        {
            var title = GetType().Name.Replace("Verb", "");
            MessageHelper.ShowMessage(title, message);
        }
    }
}
