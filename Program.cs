using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tile.Net.Layout;

namespace Tile.Net
{
    class Program
    {

        private IWorkspace _workspace;

        public static void Main(string[] args)
        {
            Win32.SetProcessDPIAware();

            new Program().Start();
        }

        void Start()
        {
            var desktopManager = WindowsDesktopManager.Instance;
            desktopManager.WindowCreated += WindowCreated;
            desktopManager.WindowDestroyed += WindowDestroyed;
            desktopManager.WindowUpdated += WindowUpdated;


            var layoutEngine = new TallLayoutEngine(1, 0.5);
            _workspace = new AllWindowWorkspace(layoutEngine);

            var msg = new Win32.Message();
            while (Win32.GetMessage(ref msg, IntPtr.Zero, 0, 0))
            {

            }
        }

        void WindowCreated(IWindow window)
        {
            _workspace.DoLayout();
        }

        void WindowDestroyed(IWindow window)
        {
            _workspace.DoLayout();
        }

        void WindowUpdated(IWindow window)
        {
            _workspace.DoLayout();
        }
    }
}
