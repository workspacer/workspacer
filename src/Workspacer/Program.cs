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
using Workspacer.Shared;

namespace Workspacer
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Win32.SetProcessDPIAware();
            Run();
        }

        private static int Run()
        {
            var app = new Workspacer();
            Thread.GetDomain().UnhandledException += ((s, e) =>
                {
                    var message = ((Exception)e.ExceptionObject).ToString() + "\n\npress ctrl-c to copy this";
                    MessageHelper.ShowMessage("unhandled exception!", message);
                    app.Quit();
                });

            app.Start();
            return 0;
        }
    }
}
