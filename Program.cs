using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    class Program
    {

        static void Main(string[] args)
        {
            var desktopManager = WindowsDesktopManager.Instance;

            var layoutEngine = new HorizontalLayoutEngine();
            var workspace = new Workspace(layoutEngine);

            var msg = new Win32.Message();
            while (Win32.GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {

            }
        }
    }
}
