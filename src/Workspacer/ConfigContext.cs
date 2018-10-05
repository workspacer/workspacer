using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private PipeServer _pipeClient;

        public ConfigContext(PipeServer pipeClient)
        {
            _pipeClient = pipeClient;

            WindowRouter = new WindowRouter(this);
        }

        private void SendResponse(LauncherResponse response)
        {
            var str = JsonConvert.SerializeObject(response);
            _pipeClient.SendResponse(str);
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
