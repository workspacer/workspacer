using AutoUpdaterDotNET;
using Microsoft.Win32;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
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
                _branch = Branch.Unstable;
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

        private static async void AutoUpdater_ParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("workspacer"));


            Release release = null;
            switch (_branch)
            {
                case Branch.Stable:
                    release = await client.Repository.Release.GetLatest("workspacer", "workspacer");
                    break;
                case Branch.Unstable:
                    release = await client.Repository.Release.Get("workspacer", "workspacer", "unstable");
                    break;
                case Branch.Beta:
                    IReadOnlyList<Release> releases =
                        await client.Repository.Release.GetAll("workspacer", "workspacer");
                    // Latest published beta release
                    release = releases.Where(r => r.TagName.Contains("-beta"))
                        .OrderByDescending(r => r.PublishedAt)
                        .First();
                    break;
            }

            string currentVersion = release.Name.Split(' ').Skip(1).FirstOrDefault();
            // If workspacer is installed and the current application is running on the install location, then use the MSI.
            string fileExtension = GetInstallLocation("workspacer", "Rick Button") == AppContext.BaseDirectory
                ? "msi"
                : "zip";

            UpdateInfoEventArgs updateInfo = new UpdateInfoEventArgs { CurrentVersion = currentVersion };
            switch (_branch)
            {
                case Branch.Stable:
                    updateInfo.ChangelogURL = "https://www.workspacer.org/changelog";
                    updateInfo.DownloadURL = release.Assets
                        .First(a => a.Name == $"workspacer-{currentVersion}-stable.{fileExtension}").BrowserDownloadUrl;
                    break;
                case Branch.Unstable:
                    updateInfo.ChangelogURL = "https://github.com/workspacer/workspacer/releases/tag/unstable";
                    updateInfo.DownloadURL = release.Assets
                        .First(a => a.Name == $"workspacer-unstable-latest.{fileExtension}").BrowserDownloadUrl;
                    break;
                case Branch.Beta:
                    updateInfo.ChangelogURL = $"https://github.com/workspacer/workspacer/releases/tag/v{currentVersion}";
                    updateInfo.DownloadURL = release.Assets
                        .First(a => a.Name == $"workspacer-{currentVersion}.{fileExtension}").BrowserDownloadUrl;
                    break;
            }

            args.UpdateInfo = updateInfo;
        }

        private static void AutoUpdater_ApplicationExitEvent()
        {
            _app.Quit();
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

        #region Helper Methods
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

        // <summary>
        // Gets the InstallLocation value of app from registry based on DisplayName and Publisher. Returns null if it does not exist.
        // </summary>
        public static string GetInstallLocation(string displayName, string publisher)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

            return key.GetSubKeyNames()
                .Select(keyName => key.OpenSubKey(keyName))
                .Where(subKey => displayName == (string)subKey?.GetValue("DisplayName")
                                 && publisher == (string)subKey?.GetValue("Publisher"))
                .Select(subKey => (string)subKey?.GetValue("InstallLocation")).FirstOrDefault();
        }
        #endregion
    }
}
