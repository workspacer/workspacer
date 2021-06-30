using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using Octokit;
using Application = System.Windows.Forms.Application;
using Timer = System.Timers.Timer;

namespace workspacer
{

    class Program
    {
        private static workspacer _app;
        private static Logger _logger = Logger.Create();
        private static Branch? _branch;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ConfigContext context = new ConfigContext();
            try
            {
                ConfigHelper.DoConfig(context);
            }
            catch
            {
                // suppress error
            }
            _branch = context.Branch;
            context.SystemTray.Dispose();

            // check for updates
            if (_branch is null)
            {
#if BRANCH_unstable
                _branch = Branch.Unstable;
#elif BRANCH_stable
                _branch = Branch.Stable;
#elif BRANCH_beta
                _branch = Branch.Beta;
#else
                _branch = Branch.None;
#endif
            }

            if (_branch != Branch.None)
            {
                AutoUpdater.RunUpdateAsAdmin = !IsDirectoryWritable(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                AutoUpdater.ParseUpdateInfoEvent += AutoUpdater_ParseUpdateInfoEvent;
                AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;

                Timer timer = new Timer(1000 * 60 * 60);
                timer.Elapsed += (s, e) =>
                {
                    AutoUpdater.Start("https://raw.githubusercontent.com/workspacer/workspacer/master/README.md");
                };
                timer.Enabled = true;
                // Put url to trigger ParseUpdateInfoEvent
                AutoUpdater.Start("https://raw.githubusercontent.com/workspacer/workspacer/master/README.md");
            }

            Run();
        }

        private static void AutoUpdater_ParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("workspacer"));

            bool isStable = _branch == Branch.Stable;
            string branchName = _branch.ToString()?.ToLower();

            Release release = isStable
                ? client.Repository.Release.GetLatest("workspacer", "workspacer").Result
                : client.Repository.Release.Get("workspacer", "workspacer", branchName).Result;

            string currentVersion = release.Name.Split(' ').Skip(1).FirstOrDefault();
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = currentVersion,
                ChangelogURL = isStable ? "https://www.workspacer.org/changelog" : $"https://github.com/workspacer/workspacer/releases/{branchName}",
                DownloadURL = release.Assets.First(a => a.Name == $"workspacer-{branchName}-{(isStable ? currentVersion : "latest")}.zip").BrowserDownloadUrl
            };
        }

        private static void AutoUpdater_ApplicationExitEvent() {
            _app.Quit();
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

        private static void Run()
        {
            _app = new workspacer();

#if !DEBUG
            Thread.GetDomain().UnhandledException += ((s, e) =>
                {
                    if (!(e.ExceptionObject is ThreadAbortException))
                    {
                        _logger.Fatal((Exception) e.ExceptionObject, "exception occurred, quiting workspacer: " + ((Exception) e.ExceptionObject).ToString());
                        _app.QuitWithException((Exception) e.ExceptionObject);
                    }
                });
#endif

            _app.Start();
        }
    }
}
