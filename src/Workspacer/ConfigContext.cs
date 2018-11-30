using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Win32;
using Newtonsoft.Json;
using Workspacer.ConfigLoader;

namespace Workspacer
{
    public class ConfigContext : IConfigContext
    {
        public IKeybindManager Keybinds { get; set; }
        public IWorkspaceManager Workspaces { get; set; }
        public IPluginManager Plugins { get; set; }
        public ISystemTrayManager SystemTray { get; set; }
        public IWindowsManager Windows { get; set; }

        public IWorkspaceContainer WorkspaceContainer { get; set; }
        public IWindowRouter WindowRouter { get; set; }

        private Timer _timer;
        private PipeServer _pipeServer;

        public ConfigContext()
        {
            _timer = new Timer();
            _timer.Elapsed += (s, e) => UpdateActiveHandles();
            _timer.Interval = 5000;
            _timer.Enabled = true;

            _pipeServer = new PipeServer();

            SystemEvents.DisplaySettingsChanged += HandleDisplaySettingsChanged;

            WindowRouter = new WindowRouter(this);
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

        public void ToggleConsoleWindow()
        {
            ConsoleHelper.ToggleConsoleWindow();
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
            Environment.Exit(0);
        }

        public void RestartAndPrompt(string message)
        {
            SaveState();
            var response = new LauncherResponse()
            {
                Action = LauncherAction.RestartAndPrompt,
                Message = message,
            };
            SendResponse(response);
            Environment.Exit(0);
        }

        public void Quit()
        {
            var response = new LauncherResponse()
            {
                Action = LauncherAction.Quit,
            };
            SendResponse(response);

            SystemTray.Destroy();
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
            foreach (var ws in WorkspaceContainer.GetAllWorkspaces())
            {
                foreach (var w in ws.Windows.Where(w => w.CanLayout))
                {
                    list.Add(w.Handle);
                }
            }
            return list;
        }

        private void HandleDisplaySettingsChanged(object sender, EventArgs e)
        {
            var message = "The display settings have changed. Workspacer will resume when you press Ok.";
            RestartAndPrompt(message);
        }

        public bool Enabled
        {
            get => Workspacer.Enabled;
            set
            {
                Workspacer.Enabled = value;
            }
        }

        private void SaveState()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Workspacer.State.json");
            var json = JsonConvert.SerializeObject(GetState());

            File.WriteAllText(filePath, json);
        }

        public WorkspacerState LoadState()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Workspacer.State.json");

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

        private object GetState()
        {
            var state = new WorkspacerState()
            {
                WorkspaceState = Workspaces.GetState()
            };
            return state;
        }
    }
}
