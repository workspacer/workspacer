using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tile.Net.Layout;

namespace Tile.Net
{
    class Program
    {

        public static void Main(string[] args)
        {
            Win32.SetProcessDPIAware();

            Thread.GetDomain().UnhandledException += (s, e) =>
            {
                WorkspaceManager.Instance.PreExitCleanup();
                Environment.Exit(1);
            };

            new Program().Start();
        }

        void Start()
        {
            WindowsDesktopManager.Instance.Initialize();

            WindowsDesktopManager.Instance.WindowCreated += WindowCreated;
            WindowsDesktopManager.Instance.WindowDestroyed += WindowDestroyed;
            WindowsDesktopManager.Instance.WindowUpdated += WindowUpdated;

            if (WindowsDesktopManager.Instance.Windows.Any())
            {
                var tempWorkspace = new Workspace(true, new TallLayoutEngine(1, 0.5, 0.03), new FullLayoutEngine());
                tempWorkspace.Show();

                foreach (var w in WindowsDesktopManager.Instance.Windows)
                {
                    tempWorkspace.WindowCreated(w);
                }
                WorkspaceManager.Instance.AddWorkspace(tempWorkspace);
            }

            WorkspaceManager.Instance.AddWorkspace(new Workspace(true, new TallLayoutEngine(1, 0.5, 0.03), new FullLayoutEngine()));

            WorkspaceManager.Instance.SwitchToWorkspace(0);

            var mod = KeyModifiers.LAlt;

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.C, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.CloseFocusedWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.Space, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.NextLayoutEngine());

            KeybindManager.Instance.Subscribe(mod, Keys.N, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.ResetLayout());

            KeybindManager.Instance.Subscribe(mod, Keys.J, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.FocusNextWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.K, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.FocusPreviousWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.M, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.FocusMasterWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.Enter, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.SwapFocusAndMasterWindow());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.J, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.SwapFocusAndNextWindow());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.K, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.SwapFocusAndPreviousWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.H, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.ShrinkMasterArea());

            KeybindManager.Instance.Subscribe(mod, Keys.L, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.ExpandMasterArea());

            KeybindManager.Instance.Subscribe(mod, Keys.Oemcomma, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.IncrementNumberOfMasterWindows());

            KeybindManager.Instance.Subscribe(mod, Keys.OemPeriod, 
                () => WorkspaceManager.Instance.FocusedWorkspace?.DecrementNumberOfMasterWindows());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.Q, Quit);

            KeybindManager.Instance.Subscribe(mod, Keys.D1, () => SwitchToWorkspace(0));
            KeybindManager.Instance.Subscribe(mod, Keys.D2, () => SwitchToWorkspace(1));
            KeybindManager.Instance.Subscribe(mod, Keys.D3, () => SwitchToWorkspace(2));
            KeybindManager.Instance.Subscribe(mod, Keys.D4, () => SwitchToWorkspace(3));
            KeybindManager.Instance.Subscribe(mod, Keys.D5, () => SwitchToWorkspace(4));
            KeybindManager.Instance.Subscribe(mod, Keys.D6, () => SwitchToWorkspace(5));
            KeybindManager.Instance.Subscribe(mod, Keys.D7, () => SwitchToWorkspace(6));
            KeybindManager.Instance.Subscribe(mod, Keys.D8, () => SwitchToWorkspace(7));
            KeybindManager.Instance.Subscribe(mod, Keys.D9, () => SwitchToWorkspace(8));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D1, () => MoveFocusedWindowToWorkspace(0));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D2, () => MoveFocusedWindowToWorkspace(1));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D3, () => MoveFocusedWindowToWorkspace(2));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D4, () => MoveFocusedWindowToWorkspace(3));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D5, () => MoveFocusedWindowToWorkspace(4));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D6, () => MoveFocusedWindowToWorkspace(5));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D7, () => MoveFocusedWindowToWorkspace(6));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D8, () => MoveFocusedWindowToWorkspace(7));
            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D9, () => MoveFocusedWindowToWorkspace(8));

            var msg = new Win32.Message();
            while (Win32.GetMessage(ref msg, IntPtr.Zero, 0, 0)) { }
        }

        void WindowCreated(IWindow window)
        {
            WorkspaceManager.Instance.AddWindow(window);
        }

        void WindowDestroyed(IWindow window)
        {
            WorkspaceManager.Instance.RemoveWindow(window);
        }

        void WindowUpdated(IWindow window)
        {
            WorkspaceManager.Instance.UpdateWindow(window);
        }

        void Quit()
        {
            WorkspaceManager.Instance.PreExitCleanup();
            Environment.Exit(0);
        }

        void SwitchToWorkspace(int index)
        {
            WorkspaceManager.Instance.SwitchToWorkspace(index);
        }

        void MoveFocusedWindowToWorkspace(int index)
        {
            WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(index);
        }
    }
}
