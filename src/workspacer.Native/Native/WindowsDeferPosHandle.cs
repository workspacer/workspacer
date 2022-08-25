using System;
using System.Collections.Generic;

namespace workspacer
{
    public class WindowsDeferPosHandle : IWindowsDeferPosHandle
    {
        private IntPtr _info;

        private List<IWindow> _toMinimize;
        private List<IWindow> _toMaximize;
        private List<IWindow> _toNormal;

        public WindowsDeferPosHandle(IntPtr info)
        {
            _info = info;
            _toMinimize = new List<IWindow>();
            _toMaximize = new List<IWindow>();
            _toNormal = new List<IWindow>();
        }

        public void Dispose()
        {
            foreach (var w in _toMinimize)
            {
                if (!w.IsMinimized)
                {
                    Win32.ShowWindow(w.Handle, Win32.SW.SW_MINIMIZE);
                }
            }
            foreach (var w in _toMaximize)
            {
                if (!w.IsMaximized)
                {
                    Win32.ShowWindow(w.Handle, Win32.SW.SW_SHOWMAXIMIZED);
                }
            }
            foreach (var w in _toNormal)
            {
                Win32.ShowWindow(w.Handle, Win32.SW.SW_SHOWNOACTIVATE);
            }

            Win32.EndDeferWindowPos(_info);
        }

        public void DeferWindowPos(IWindow window, IWindowLocation location)
        {
            var flags = Win32.SWP.SWP_FRAMECHANGED | Win32.SWP.SWP_NOACTIVATE | Win32.SWP.SWP_NOCOPYBITS |
                        Win32.SWP.SWP_NOZORDER | Win32.SWP.SWP_NOOWNERZORDER;

            if (location.State == WindowState.Maximized)
            {
                _toMaximize.Add(window);
                flags = flags | Win32.SWP.SWP_NOMOVE | Win32.SWP.SWP_NOSIZE;
            }
            else if (location.State == WindowState.Minimized)
            {
                _toMinimize.Add(window);
                flags = flags | Win32.SWP.SWP_NOMOVE | Win32.SWP.SWP_NOSIZE;
            }
            else
            {
                _toNormal.Add(window);
            }

            // Calculate final position for window
            var offset = window.Offset;
            int X = location.X + offset.X;
            int Y = location.Y + offset.Y;
            int Width = location.Width + offset.Width;
            int Height = location.Height + offset.Height;

            Win32.DeferWindowPos(_info, window.Handle, IntPtr.Zero, X, Y, Width, Height, flags);
        }
    }
}
