using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer
{
    public static class FocusStealer
    {
        private static Logger Logger = Logger.Create();

        public static void Steal(IntPtr windowToFocus)
        {
            Win32.AllocConsole();
            var hWndConsole = Win32.GetConsoleWindow();
            Win32.SetWindowPos(hWndConsole, IntPtr.Zero, 0, 0, 0, 0, Win32.SetWindowPosFlags.IgnoreZOrder);
            Win32.FreeConsole();
            Win32.SetForegroundWindow(windowToFocus);
        }
    }
}
