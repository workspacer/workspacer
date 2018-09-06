using System;
using System.Linq;
using Newtonsoft.Json;
using Tile.Net.Shared;
using Tile.Net.ConfigLoader;
using Tile.Net.Plugins;
using Timer = System.Timers.Timer;

namespace Tile.Net
{
    public class TileNet
    {
        public static bool Enabled { get; set; }

        private PipeClient _pipeClient;
        private ConfigContext _context;
        private Timer _timer;

        public TileNet(string clientHandle)
        {
            _pipeClient = new PipeClient(clientHandle);
            _timer = new Timer();
            _timer.Elapsed += (s, e) => UpdateActiveHandles();
            _timer.Interval = 5000;
        }

        public TileNet() : this(null)
        {

        }

        public void Start()
        {
            _pipeClient.Start();
            _timer.Enabled = true;

            WindowsDesktopManager.Instance.WindowCreated += WorkspaceManager.Instance.AddWindow;
            WindowsDesktopManager.Instance.WindowDestroyed += WorkspaceManager.Instance.RemoveWindow;
            WindowsDesktopManager.Instance.WindowUpdated += WorkspaceManager.Instance.UpdateWindow;

            PluginManager.Instance.BeforeConfig();

            _context = new ConfigContext(_pipeClient)
            {
                Keybinds = KeybindManager.Instance,
                Workspaces = WorkspaceManager.Instance,
                Layouts = LayoutManager.Instance,
            };
            var config = GetConfig();
            config.Configure(_context);

            PluginManager.Instance.AfterConfig();

            var state = StateManager.Instance.LoadState();

            WindowsDesktopManager.Instance.Initialize();
            if (state != null)
            {
                WorkspaceManager.Instance.InitializeWithState(state.WorkspaceState, WindowsDesktopManager.Instance.Windows);
                Enabled = true;
            }
            else
            {
                WorkspaceManager.Instance.Initialize(WindowsDesktopManager.Instance.Windows);
                Enabled = true;
                WorkspaceManager.Instance.SwitchToWorkspace(0);
            }


            var msg = new Win32.Message();
            while (Win32.GetMessage(ref msg, IntPtr.Zero, 0, 0)) { }
        }

        private IConfig GetConfig()
        {
            return ConfigHelper.GetConfig();
        }

        private void SendResponse(LauncherResponse response)
        {
            var str = JsonConvert.SerializeObject(response);
            _pipeClient.SendResponse(str);
        }

        public void Quit()
        {
            _context.Quit();
        }

        public void UpdateActiveHandles()
        {
            var response = new LauncherResponse()
            {
                Action = LauncherAction.UpdateHandles,
                ActiveHandles = WorkspaceManager.Instance.GetActiveHandles().Select(h => h.ToInt64()).ToList(),
            };
            SendResponse(response);
        }
    }
}
