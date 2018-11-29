using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workspacer
{
    class Program
    {
        private static Logger Logger = Logger.Create();

        [STAThread]
        public static void Main(string[] args)
        {
            Win32.SetProcessDPIAware();

#if BRANCH_unstable
            var branch = "unstable";
#elif BRANCH_stable 
            var branch = "stable";
#else
            string branch = null;
#endif
            if (branch != null)
            {
                var xmlUrl = $"https://workspacer.blob.core.windows.net/installers/{branch}.xml";

                AutoUpdater.ApplicationExitEvent += QuitForUpdate;

                System.Timers.Timer timer = new System.Timers.Timer(1000 * 60 * 60);
                timer.Elapsed += (s, e) =>
                {
                    AutoUpdater.Start(xmlUrl);
                };
                timer.Enabled = true;
                AutoUpdater.Start(xmlUrl);
            }

            Run();
        }

        private static Workspacer _app;

        private static int Run()
        {
            _app = new Workspacer();
            Thread.GetDomain().UnhandledException += ((s, e) =>
                {
                    var exceptionMessage = ((Exception)e.ExceptionObject).ToString();
                    Logger.Fatal(exceptionMessage);

                    var message = exceptionMessage + "\n\npress ctrl-c to copy this";
                    MessageHelper.ShowMessage("unhandled exception!", message);
                    _app.Quit();
                });

            _app.Start();
            return 0;
        }

        private static void QuitForUpdate()
        {
            _app.Quit();
        }
    }
}
