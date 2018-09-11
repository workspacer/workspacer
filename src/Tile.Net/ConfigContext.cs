using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tile.Net.ConfigLoader;
using Tile.Net.Shared;

namespace Tile.Net
{
    public class ConfigContext : IConfigContext
    {
        public IKeybindManager Keybinds { get; set; }
        public ILayoutManager Layouts { get; set; }
        public IWorkspaceManager Workspaces { get; set; }
        public IPluginManager Plugins { get; set; }

        private PipeServer _pipeClient;

        public ConfigContext(PipeServer pipeClient)
        {
            _pipeClient = pipeClient;
        }

        private void SendResponse(LauncherResponse response)
        {
            var str = JsonConvert.SerializeObject(response);
            _pipeClient.SendResponse(str);
        }
        
        public void Restart()
        {
            StateManager.Instance.SaveState();
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
            Environment.Exit(0);
        }

        public bool Enabled
        {
            get => TileNet.Enabled;
            set
            {
                TileNet.Enabled = value;
            }
        }
    }
}
