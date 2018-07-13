using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile.Net
{
    class Program
    {
        public static void Main(string[] args)
        {
            Win32.SetProcessDPIAware();
            Thread.GetDomain().UnhandledException += ((s, e) =>
                {
                    Console.Write(e.ExceptionObject);
                    TileNet.Instance.Quit(1);
                });

            TileNet.Instance.Start();
        }
    }
}
