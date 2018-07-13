using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class WindowsWindow : IWindow
    {
        private IntPtr _handle;
        private bool _layoutOverride;

        public WindowsWindow(IntPtr handle)
        {
            _handle = handle;
        }

        public string Title
        {
            get
            {
                var buffer = new StringBuilder(255);
                Win32.GetWindowText(_handle, buffer, buffer.Capacity + 1);
                return buffer.ToString();
            }
        }

        public IntPtr Handle => _handle;

        public IWindowLocation Location
        {
            get
            {
                Win32.Rect rect = new Win32.Rect();
                Win32.GetWindowRect(_handle, ref rect);

                WindowState state = WindowState.Normal;
                if (IsMinimized)
                {
                    state = WindowState.Minimized;
                }
                else if (IsMaximized)
                {
                    state = WindowState.Maximized;
                }

                return new WindowLocation(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, state);
            }
        }

        public Process Process
        {
            get
            {
                try
                {
                    uint processId;
                    var threadId = Win32.GetWindowThreadProcessId(_handle, out processId);
                    return Process.GetProcessById((int) processId);
                }
                catch (Win32Exception e)
                {
                    return null;
                }
            }
        }

        public string ProcessFileName
        {
            get
            {
                try
                {
                    return Process != null ? Path.GetFileName(Process.MainModule.FileName) : null;
                }
                catch (Win32Exception e)
                {
                    return null;
                }
            }
        }

        public bool CanLayout
        {
            get
            {
                return _layoutOverride || 
                    (!Win32Helper.IsCloaked(_handle) &&
                       Win32Helper.IsAppWindow(_handle) &&
                       Win32Helper.IsAltTabWindow(_handle));
            }
        }

        public bool IsFocused
        {
            get { return Win32.GetForegroundWindow() == _handle; }
            set { Win32.SetForegroundWindow(_handle); }
        }

        public bool IsMinimized => Win32.IsIconic(_handle);
        public bool IsMaximized => Win32.IsZoomed(_handle);

        public bool CanResize
        {
            get
            {
                return Win32.GetWindowStyleLongPtr(_handle).HasFlag(Win32.WS.WS_SIZEBOX);
            }
            set
            {
                var style = Win32.GetWindowStyleLongPtr(_handle);
                style = value ? style | Win32.WS.WS_SIZEBOX : style & ~Win32.WS.WS_SIZEBOX;
                Win32.SetWindowLongPtr(_handle, Win32.GWL_STYLE, (uint)style);
            }
        }

        public void Hide()
        {
            if (CanLayout)
            {
                _layoutOverride = true;
            }
            Win32.ShowWindow(_handle, Win32.SW.SW_HIDE);
        }

        public void ShowNormal()
        {
            Win32.ShowWindow(_handle, Win32.SW.SW_SHOWNOACTIVATE);
        }

        public void ShowMaximized()
        {
            Win32.ShowWindow(_handle, Win32.SW.SW_SHOWMAXIMIZED);
        }

        public void ShowMinimized()
        {
            Win32.ShowWindow(_handle, Win32.SW.SW_SHOWMINIMIZED);
        }

        public void ShowInCurrentState()
        {
            if (IsMinimized)
            {
                ShowMinimized();
            }
            else if (IsMaximized)
            {
                ShowMaximized();
            }
            else
            {
                ShowNormal();
            }
        }

        public void Close()
        {
            Win32Helper.QuitApplication(_handle);
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
