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
            _workspace = new AllWindowWorkspace(new TallLayoutEngine(1, 0.5, 0.03), new FullLayoutEngine());

            WindowsDesktopManager.Instance.WindowCreated += WindowCreated;
            WindowsDesktopManager.Instance.WindowDestroyed += WindowDestroyed;
            WindowsDesktopManager.Instance.WindowUpdated += WindowUpdated;

            WindowsDesktopManager.Instance.Initialize();

            var mod = KeyModifiers.LAlt;

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.C, () => _workspace.CloseFocusedWindow());
            KeybindManager.Instance.Subscribe(mod, Keys.Space, () => _workspace.NextLayoutEngine());
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.Space, () => _workspace.ResetLayout());

            KeybindManager.Instance.Subscribe(mod, Keys.J, () => _workspace.FocusNextWindow());
            KeybindManager.Instance.Subscribe(mod, Keys.K, () => _workspace.FocusPreviousWindow());
            KeybindManager.Instance.Subscribe(mod, Keys.M, () => _workspace.FocusMasterWindow());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.Enter, () => _workspace.SwapFocusAndMasterWindow());
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.J, () => _workspace.SwapFocusAndNextWindow());
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.K, () => _workspace.SwapFocusAndPreviousWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.H, () => _workspace.ShrinkMasterArea());
            KeybindManager.Instance.Subscribe(mod, Keys.L, () => _workspace.ExpandMasterArea());

            KeybindManager.Instance.Subscribe(mod, Keys.Oemcomma, () => _workspace.IncrementNumberOfMasterWindows());
            KeybindManager.Instance.Subscribe(mod, Keys.OemPeriod, () => _workspace.DecrementNumberOfMasterWindows());

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
