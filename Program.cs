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
            desktopManager.WindowCreated += WindowCreated;
            desktopManager.WindowDestroyed += WindowDestroyed;
            desktopManager.WindowUpdated += WindowUpdated;


            var layoutEngine = new HorizontalLayoutEngine();
            var workspace = new Workspace(layoutEngine);

            var msg = new Win32.Message();
            while (Win32.GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {

            }
        }

        static void WindowCreated(IWindow window)
        {
            Output();
        }

        static void WindowDestroyed(IWindow window)
        {
            Output();
        }

        static void WindowUpdated(IWindow window)
        {
            Output();
        }

        static void Output()
        {
            Console.Clear();
            foreach (var w in WindowsDesktopManager.Instance.Windows)
            {
                if (w.CanLayout)
                {
                    Console.WriteLine(w.Title);
                }
            }
        }
    }
}
