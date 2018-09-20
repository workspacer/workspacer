using CommandLine;
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
using Tile.Net.Shared;
using Tile.Net.Verbs;

namespace Tile.Net
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Win32.SetProcessDPIAware();

            if (args.Length > 0)
            {
                CommandLine.Parser.Default.ParseArguments<InitVerbOptions>(args)
                    .MapResult((InitVerbOptions opts) => new InitVerb().Execute(),
                    (errs) => 1);
            } else
            {
                Run();
            }
        }

        private static int Run()
        {
            var app = new TileNet();
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
