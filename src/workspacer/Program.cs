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
        private static Logger Logger = Logger.Create();
        private static readonly ConfigContext Context = new ConfigContext();

        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                ConfigHelper.DoConfig(Context);
            }
            catch
            {
                // suppress error
            }

            // check for updates
            if (Context.Branch is null)
            {
#if BRANCH_unstable
                Context.Branch = Branch.Unstable;
#elif BRANCH_stable
                Context.Branch = Branch.Stable;
#else
                Context.Branch = Branch.None;
#endif
            }

            if (Context.Branch != Branch.None)
            {
                AutoUpdater.RunUpdateAsAdmin = !IsDirectoryWritable(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                AutoUpdater.ParseUpdateInfoEvent += AutoUpdater_ParseUpdateInfoEvent;
                AutoUpdater.ApplicationExitEvent += QuitForUpdate;

                Timer timer = new Timer(1000 * 60 * 60);
                timer.Elapsed += (s, e) =>
                {
                    AutoUpdater.Start("https://raw.githubusercontent.com/rickbutton/workspacer/master/README.md");
                };
                timer.Enabled = true;
                // Put url to trigger ParseUpdateInfoEvent
                AutoUpdater.Start("https://raw.githubusercontent.com/rickbutton/workspacer/master/README.md");
            }

            Run();
        }

        private static void AutoUpdater_ParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("workspacer"));

            bool isStable = _branch == Branch.Stable;

            Release release = isStable
                ? client.Repository.Release.GetLatest("rickbutton", "workspacer").Result
                : client.Repository.Release.Get("rickbutton", "workspacer", "unstable").Result;

            string currentVersion = release.Name.Split(' ').Skip(1).FirstOrDefault();
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = currentVersion,
                ChangelogURL = isStable ? "https://www.workspacer.org/changelog" : "https://github.com/rickbutton/workspacer/releases/unstable",
                DownloadURL = release.Assets.First(a => a.Name == $"workspacer-{_branch.ToString()?.ToLower()}-{(isStable ? currentVersion : "latest")}.zip").BrowserDownloadUrl
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

        private static void Run()
        {
            _app = new workspacer();

#if !DEBUG
            Thread.GetDomain().UnhandledException += ((s, e) =>
                {
                    if (!(e.ExceptionObject is ThreadAbortException))
                    {
                        Logger.Fatal(e.ExceptionObject as Exception, "exception occurred, quiting workspacer: " + (e.ExceptionObject as Exception).ToString());
                        _app.QuitWithException(e.ExceptionObject as Exception);
                    }
                });
#endif

            _app.Start();
        }

        private static void QuitForUpdate()
        {
            _app.Quit();
        }
    }
}
