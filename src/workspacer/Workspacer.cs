using System;
using System.Linq;
using Timer = System.Timers.Timer;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using Octokit;
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
            FileHelper.EnsureUserWorkspacerPathExists();

            // init logging
            Logger.Initialize(FileHelper.GetUserWorkspacerPath());
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
            _context.SystemTray.AddToContextMenu("enable/disable workspacer", () => _context.Enabled = !_context.Enabled);
            _context.SystemTray.AddToContextMenu("quit workspacer", () => _context.Quit());
            _context.SystemTray.AddToContextMenu("restart workspacer", () => _context.Restart());
            _context.SystemTray.AddToContextMenu("show/hide keybind help", () => _context.Keybinds.ShowKeybindDialog());
            if (ConfigHelper.CanCreateExampleConfig())
            {
                _context.SystemTray.AddToContextMenu("create example workspacer.config.csx", CreateExampleConfig);
            }

            // init config
            ConfigHelper.DoConfig(_context);

            // check for updates
            if (_context.Branch is null)
            {
#if BRANCH_unstable
                _context.Branch = Branch.Unstable;
#elif BRANCH_stable
                _context.Branch = Branch.Stable;
#else
                _context.Branch = Branch.None;
#endif
            }

            if (_context.Branch != Branch.None)
            {
                if (!IsDirectoryWritable(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
                {
                    AutoUpdater.RunUpdateAsAdmin = true;
                }
                AutoUpdater.ParseUpdateInfoEvent += AutoUpdater_ParseUpdateInfoEvent;
                AutoUpdater.ApplicationExitEvent += Quit;

                Timer timer = new Timer(1000 * 60 * 60);
                timer.Elapsed += (s, e) =>
                {
                    AutoUpdater.Start("https://raw.githubusercontent.com/rickbutton/workspacer/master/README.md");
                };
                timer.Enabled = true;
                // Put url to trigger ParseUpdateInfoEvent
                AutoUpdater.Start("https://raw.githubusercontent.com/rickbutton/workspacer/master/README.md");
            }

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

        private void AutoUpdater_ParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("workspacer"));

            Release release = _context.Branch == Branch.Stable
                ? client.Repository.Release.GetLatest("rickbutton", "workspacer").Result
                : client.Repository.Release.Get("rickbutton", "workspacer", "Unstable").Result;

            string currentVersion = release.Name.Split(' ').Skip(1).FirstOrDefault();
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = currentVersion,
                ChangelogURL = "https://www.workspacer.org/changelog",
                DownloadURL = release.Assets.FirstOrDefault(a => a.Name == $"workspacer-{_context.Branch.ToString()?.ToLower()}-{currentVersion}.zip").BrowserDownloadUrl
            };
        }

        private static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }

                return true;
            }
            catch
            {
                if (throwIfFails) throw;

                return false;
            }
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
                return Assembly.LoadFile(match.Location);
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
                DisplayMessage($"workspacer.config.csx created in: [${FileHelper.GetUserWorkspacerPath()}]");
            }
        }

        public void DisplayMessage(string message)
        {
            MessageBox.Show(message, "workspacer");
        }
    }
}
