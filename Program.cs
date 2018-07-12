using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            var layoutEngine = new TallLayoutEngine(1, 0.5);
            _workspace = new AllWindowWorkspace(layoutEngine);

            WindowsDesktopManager.Instance.WindowCreated += WindowCreated;
            WindowsDesktopManager.Instance.WindowDestroyed += WindowDestroyed;
            WindowsDesktopManager.Instance.WindowUpdated += WindowUpdated;

            WindowsDesktopManager.Instance.Initialize();

            var mod = KeyModifiers.LAlt | KeyModifiers.LWin;
            KeybindManager.Instance.Subscribe(mod, Keys.J, () => _workspace.FocusNextWindow());
            KeybindManager.Instance.Subscribe(mod, Keys.K, () => _workspace.FocusPreviousWindow());
            KeybindManager.Instance.Subscribe(mod, Keys.M, () => _workspace.FocusMasterWindow());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.Enter, () => _workspace.SwapFocusAndMasterWindow());
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.J, () => _workspace.SwapFocusAndNextWindow());
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.K, () => _workspace.SwapFocusAndPreviousWindow());

            var msg = new Win32.Message();
            while (Win32.GetMessage(ref msg, IntPtr.Zero, 0, 0)) { }
        }

        void WindowCreated(IWindow window)
        {
            _workspace.WindowCreated(window);
        }

        void WindowDestroyed(IWindow window)
        {
            _workspace.WindowDestroyed(window);
        }

        void WindowUpdated(IWindow window)
        {
            _workspace.WindowUpdated(window);
        }
    }
}
