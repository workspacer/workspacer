using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Native
{
    public class WindowsDeferPosHandle : IDisposable
    {
        private IntPtr _info;

        public WindowsDeferPosHandle(IntPtr info)
        {
            _info = info;
        }

        public void Dispose()
        {
            Win32.EndDeferWindowPos(_info);
        }

        public void DeferWindowPos(IWindow window, int x, int y, int cx, int cy)
        {
            Win32.DeferWindowPos(_info, window.Handle, IntPtr.Zero, x, y, cx, cy, 
                Win32.SWP.SWP_FRAMECHANGED | Win32.SWP.SWP_NOACTIVATE | Win32.SWP.SWP_NOCOPYBITS |
				Win32.SWP.SWP_NOZORDER | Win32.SWP.SWP_NOOWNERZORDER);
        }
    }
}
