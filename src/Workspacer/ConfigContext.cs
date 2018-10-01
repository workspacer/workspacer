using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Workspacer.ConfigLoader;
using Workspacer.Shared;

namespace Workspacer
{
    public class ConfigContext : IConfigContext
    {
        public IKeybindManager Keybinds { get; set; }
        public IWorkspaceManager Workspaces { get; set; }
        public IPluginManager Plugins { get; set; }
        public ISystemTrayManager SystemTray { get; set; }
        public IWindowsManager Windows { get; set; }

        private PipeServer _pipeClient;
        private StateManager _state;

        public ConfigContext(PipeServer pipeClient, StateManager state)
        {
            _pipeClient = pipeClient;
            _state = state;
        }

        private void SendResponse(LauncherResponse response)
        {
            var str = JsonConvert.SerializeObject(response);
            _pipeClient.SendResponse(str);
        }
        
        public void Restart()
        {
            _state.SaveState();
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
    }
}
