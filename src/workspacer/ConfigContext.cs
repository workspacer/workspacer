using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace workspacer
{
    public class ConfigContext : IConfigContext
    {
        public KeybindManager Keybinds { get; set; }
        IKeybindManager IConfigContext.Keybinds { get { return Keybinds; } }

        public WorkspaceManager Workspaces { get; set; }
        IWorkspaceManager IConfigContext.Workspaces { get { return Workspaces; } }

        public PluginManager Plugins { get; set; }
        IPluginManager IConfigContext.Plugins { get { return Plugins; } }

        public SystemTrayManager SystemTray { get; set; }
        ISystemTrayManager IConfigContext.SystemTray { get { return SystemTray; } }

        public WindowsManager Windows { get; set; }
        IWindowsManager IConfigContext.Windows { get { return Windows; } }

        public IWorkspaceContainer WorkspaceContainer { get; set; }
        public IWindowRouter WindowRouter { get; set; }
        public IMonitorContainer MonitorContainer { get; set; }

        private System.Timers.Timer _timer;
        private PipeServer _pipeServer;
        private Func<ILayoutEngine[]> _defaultLayouts;
        private List<Func<ILayoutEngine, ILayoutEngine>> _layoutProxies;

        public ConfigContext()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += (s, e) => UpdateActiveHandles();
            _timer.Interval = 5000;
            _timer.Enabled = true;

            _pipeServer = new PipeServer();

            _defaultLayouts = () => new ILayoutEngine[] {
                new TallLayoutEngine(),
                new FullLayoutEngine()
            };
            _layoutProxies = new List<Func<ILayoutEngine, ILayoutEngine>>();

            SystemEvents.DisplaySettingsChanged += HandleDisplaySettingsChanged;

            Plugins = new PluginManager();
            SystemTray = new SystemTrayManager();
            Workspaces = new WorkspaceManager(this);
            Windows = new WindowsManager();
            Keybinds = new KeybindManager(this);

            WorkspaceContainer = new WorkspaceContainer(this);
            WindowRouter = new WindowRouter(this);
            MonitorContainer = new NativeMonitorContainer();

            Windows.WindowCreated += Workspaces.AddWindow;
            Windows.WindowDestroyed += Workspaces.RemoveWindow;
            Windows.WindowUpdated += Workspaces.UpdateWindow;

            // ignore watcher windows in workspacer
            WindowRouter.AddFilter((window) => !(window.ProcessId == _pipeServer.WatcherProcess.Id));
        }

        public void ConnectToWatcher()
        {
            _pipeServer.Start();
        }

        public LogLevel ConsoleLogLevel
        {
            get
            {
                return Logger.ConsoleLogLevel;
            }
            set
            {
                Logger.ConsoleLogLevel = value;
            }
        }

        public LogLevel FileLogLevel
        {
            get
            {
                return Logger.FileLogLevel;
            }
            set
            {
                Logger.FileLogLevel = value;
            }
        }

        public Func<ILayoutEngine[]> DefaultLayouts
        {
            get
            {
                return () => ProxyLayouts(_defaultLayouts()).ToArray();
            }
            set
            {
                _defaultLayouts = value;
            }
        }

        public void AddLayoutProxy(Func<ILayoutEngine, ILayoutEngine> proxy)
        {
            _layoutProxies.Add(proxy);
        }

        public IEnumerable<ILayoutEngine> ProxyLayouts(IEnumerable<ILayoutEngine> layouts)
        {
            for (var i = 0; i < _layoutProxies.Count; i++)
            {
                layouts = layouts.Select(layout => _layoutProxies[i](layout)).ToArray();
            }
            return layouts;
        }

        public void ToggleConsoleWindow()
        {
            var response = new LauncherResponse()
            {
                Action = LauncherAction.ToggleConsole,
            };
            SendResponse(response);
        }

        public void SendLogToConsole(string message)
        {
            var response = new LauncherResponse()
            {
                Action = LauncherAction.Log,
                Message = message,
            };
            SendResponse(response);
        }

        private void SendResponse(LauncherResponse response)
        {
            var str = JsonConvert.SerializeObject(response);
            _pipeServer.SendResponse(str);
        }
        
        public void Restart()
        {
            SaveState();
            var response = new LauncherResponse()
            {
                Action = LauncherAction.Restart,
            };
            SendResponse(response);

            CleanupAndExit();
        }

        public void Quit()
        {
            var response = new LauncherResponse()
            {
                Action = LauncherAction.Quit,
            };
            SendResponse(response);

            CleanupAndExit();
        }

        public void QuitWithException(Exception e)
        {
            var message = e.ToString();
            var response = new LauncherResponse()
            {
                Action = LauncherAction.QuitWithException,
                Message = message,
            };
            SendResponse(response);

            CleanupAndExit();
        }

        public void CleanupAndExit()
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void UpdateActiveHandles()
        {
            var response = new LauncherResponse()
            {
                Action = LauncherAction.UpdateHandles,
                ActiveHandles = GetActiveHandles().Select(h => h.ToInt64()).ToList(),
            };
            SendResponse(response);
        }

        private List<IntPtr> GetActiveHandles()
        {
            var list = new List<IntPtr>();
            if (WorkspaceContainer == null) return list;

            foreach (var ws in WorkspaceContainer.GetAllWorkspaces())
            {
                var handles = ws.ManagedWindows.Select(i => i.Handle);
                list.AddRange(handles);
            }
            return list;
        }

        private void HandleDisplaySettingsChanged(object sender, EventArgs e)
        {
            SaveState();
            var response = new LauncherResponse()
            {
                Action = LauncherAction.RestartWithMessage,
                Message = "A display settings change has been detected, which has automatically disabled workspacer. Press 'restart' when ready.",
            };
            SendResponse(response);

            CleanupAndExit();
        }

        public bool Enabled
        {
            get => workspacer.Enabled;
            set
            {
                workspacer.Enabled = value;
            }
        }

        private void SaveState()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "workspacer.State.json");
            var json = JsonConvert.SerializeObject(GetState());

            File.WriteAllText(filePath, json);
        }

        public WorkspacerState LoadState()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "workspacer.State.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var state = JsonConvert.DeserializeObject<WorkspacerState>(json);
                File.Delete(filePath);
                return state;
            }
            else
            {
                return null;
            }
        }

        private WorkspacerState GetState()
        {
            var state = new WorkspacerState()
            {
                WorkspaceState = Workspaces.GetState()
            };
            return state;
        }
    }
}
