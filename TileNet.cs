using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile.Net
{
    public class TileNet
    {
        public static TileNet Instance { get; } = new TileNet();

        public bool Enabled { get; set; }

        private TileNet()
        {
        }

        public void Start()
        {
            DoConfig();

            WindowsDesktopManager.Instance.Initialize();
            foreach (var w in WindowsDesktopManager.Instance.Windows)
            {
                WorkspaceManager.Instance.AddWindow(w, false);
            }

            Enabled = true;
            WorkspaceManager.Instance.SwitchToWorkspace(0);

            var msg = new Win32.Message();
            while (Win32.GetMessage(ref msg, IntPtr.Zero, 0, 0)) { }
        }

        void DoConfig()
        {
            var mod = KeyModifiers.LAlt;

            WindowsDesktopManager.Instance.WindowCreated += WorkspaceManager.Instance.AddWindow;
            WindowsDesktopManager.Instance.WindowDestroyed += WorkspaceManager.Instance.RemoveWindow;
            WindowsDesktopManager.Instance.WindowUpdated += WorkspaceManager.Instance.UpdateWindow;

            LayoutManager.Instance.AddLayout<TallLayoutEngine>(1, 0.5, 0.03);
            LayoutManager.Instance.AddLayout<FullLayoutEngine>();

            WorkspaceManager.Instance.WorkspaceSelectorFunc = (window) =>
            {
                if (KeybindManager.Instance.KeyIsPressed(Keys.LMenu))
                {
                    return WorkspaceManager.Instance.FocusedWorkspace;
                }

                if (window.ProcessFileName != null)
                {
                    if (window.ProcessFileName == "chrome.exe")
                    {
                        return WorkspaceManager.Instance["web"];
                    }
                    if (window.ProcessFileName == "ConEmu64.exe")
                    {
                        return WorkspaceManager.Instance["term"];
                    }
                    if (window.ProcessFileName == "devenv.exe")
                    {
                        return WorkspaceManager.Instance["code"];
                    }
                }

                return WorkspaceManager.Instance.FocusedWorkspace;
            };

            WorkspaceManager.Instance.AddWorkspace("main", LayoutManager.Instance.CreateLayouts());
            WorkspaceManager.Instance.AddWorkspace("web", LayoutManager.Instance.CreateLayouts());
            WorkspaceManager.Instance.AddWorkspace("code", LayoutManager.Instance.CreateLayouts());
            WorkspaceManager.Instance.AddWorkspace("term", LayoutManager.Instance.CreateLayouts());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.C, 
                () => WorkspaceManager.Instance.FocusedWorkspace.CloseFocusedWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.Space, 
                () => WorkspaceManager.Instance.FocusedWorkspace.NextLayoutEngine());

            KeybindManager.Instance.Subscribe(mod, Keys.N, 
                () => WorkspaceManager.Instance.FocusedWorkspace.ResetLayout());

            KeybindManager.Instance.Subscribe(mod, Keys.J, 
                () => WorkspaceManager.Instance.FocusedWorkspace.FocusNextWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.K, 
                () => WorkspaceManager.Instance.FocusedWorkspace.FocusPreviousWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.M, 
                () => WorkspaceManager.Instance.FocusedWorkspace.FocusMasterWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.Enter, 
                () => WorkspaceManager.Instance.FocusedWorkspace.SwapFocusAndMasterWindow());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.J, 
                () => WorkspaceManager.Instance.FocusedWorkspace.SwapFocusAndNextWindow());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.K, 
                () => WorkspaceManager.Instance.FocusedWorkspace.SwapFocusAndPreviousWindow());

            KeybindManager.Instance.Subscribe(mod, Keys.H, 
                () => WorkspaceManager.Instance.FocusedWorkspace.ShrinkMasterArea());

            KeybindManager.Instance.Subscribe(mod, Keys.L, 
                () => WorkspaceManager.Instance.FocusedWorkspace.ExpandMasterArea());

            KeybindManager.Instance.Subscribe(mod, Keys.Oemcomma, 
                () => WorkspaceManager.Instance.FocusedWorkspace.IncrementNumberOfMasterWindows());

            KeybindManager.Instance.Subscribe(mod, Keys.OemPeriod, 
                () => WorkspaceManager.Instance.FocusedWorkspace.DecrementNumberOfMasterWindows());

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.Q, () => Quit(0));

            KeybindManager.Instance.Subscribe(mod, Keys.D1, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(0));

            KeybindManager.Instance.Subscribe(mod, Keys.D2, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(1));

            KeybindManager.Instance.Subscribe(mod, Keys.D3, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(2));

            KeybindManager.Instance.Subscribe(mod, Keys.D4, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(3));

            KeybindManager.Instance.Subscribe(mod, Keys.D5, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(4));

            KeybindManager.Instance.Subscribe(mod, Keys.D6, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(5));

            KeybindManager.Instance.Subscribe(mod, Keys.D7, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(6));

            KeybindManager.Instance.Subscribe(mod, Keys.D8, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(7));

            KeybindManager.Instance.Subscribe(mod, Keys.D9, 
                () => WorkspaceManager.Instance.SwitchToWorkspace(8));


            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D1, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(0));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D2, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(1));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D3, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(2));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D4, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(3));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D5, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(4));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D6, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(5));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D7, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(6));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D8, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(7));

            KeybindManager.Instance.Subscribe(mod | KeyModifiers.LShift, Keys.D9, 
                () => WorkspaceManager.Instance.MoveFocusedWindowToWorkspace(8));
        }

        public void Quit(int exit)
        {
            WorkspaceManager.Instance.PreExitCleanup();
            Environment.Exit(exit);
        }
    }
}
