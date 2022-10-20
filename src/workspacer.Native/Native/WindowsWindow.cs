using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace workspacer
{
    public class WindowsWindow : IWindow
    {
        private static Logger Logger = Logger.Create();

        private IntPtr _handle;
        private bool _didManualHide;

        public event IWindowDelegate WindowClosed;
        public event IWindowDelegate WindowUpdated;
        public event IWindowDelegate WindowFocused;

        private int _processId;
        private string _processName;
        private string _processFileName;

        public WindowsWindow(IntPtr handle)
        {
            _handle = handle;

            try
            {
                uint processId;
                Win32.GetWindowThreadProcessId(_handle, out processId);

                _processId = (int)processId;

                var process = Process.GetProcesses().FirstOrDefault(p => p.Id == _processId);
                _processName = process.ProcessName;

                try
                {
                    _processFileName = Path.GetFileName(process.MainModule.FileName);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    _processFileName = "--NA--";
                }
            }
            catch (Exception)
            {
                _processId = -1;
                _processName = "";
                _processFileName = "";
            }
        }

        public bool DidManualHide => _didManualHide;

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

        public string Class
        {
            get
            {
                var buffer = new StringBuilder(255);
                Win32.GetClassName(_handle, buffer, buffer.Capacity + 1);
                return buffer.ToString();
            }
        }

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

        public Rectangle Offset
        {
            get
            {
                // Window Rect via GetWindowRect
                Win32.Rect rect1 = new Win32.Rect();
                Win32.GetWindowRect(_handle, ref rect1);

                int X1 = rect1.Left;
                int Y1 = rect1.Top;
                int Width1 = rect1.Right - rect1.Left;
                int Height1 = rect1.Bottom - rect1.Top;

                // Window Rect via DwmGetWindowAttribute
                Win32.Rect rect2 = new Win32.Rect();
                int size = Marshal.SizeOf(typeof(Win32.Rect));
                Win32.DwmGetWindowAttribute(_handle, (int)Win32.DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out rect2, size);

                int X2 = rect2.Left;
                int Y2 = rect2.Top;
                int Width2 = rect2.Right - rect2.Left;
                int Height2 = rect2.Bottom - rect2.Top;

                // Calculate offset
                int X = X1 - X2;
                int Y = Y1 - Y2;
                int Width = Width1 - Width2;
                int Height = Height1 - Height2;

                return new Rectangle(X, Y, Width, Height);
            }
        }

        public int ProcessId => _processId;
        public string ProcessFileName => _processFileName;
        public string ProcessName => _processName;

        public bool CanLayout
        {
            get
            {
                return _didManualHide ||
                    (!Win32Helper.IsCloaked(_handle) &&
                       Win32Helper.IsAppWindow(_handle) &&
                       Win32Helper.IsAltTabWindow(_handle));
            }
        }


        public bool IsFocused => Win32.GetForegroundWindow() == _handle;
        public bool IsMinimized => Win32.IsIconic(_handle);
        public bool IsMaximized => Win32.IsZoomed(_handle);

        public bool IsFullscreen
        {
            get
            {
                Win32.Rect rect = new Win32.Rect();
                Rectangle screenrect = Screen.FromHandle(_handle).Bounds;
                Win32.GetWindowRect(_handle,ref rect);
                return rect.Left == screenrect.Left && rect.Right == screenrect.Right && rect.Top == screenrect.Top && rect.Bottom == screenrect.Bottom;
            }
        }

        public bool IsMouseMoving { get; internal set; }

        public void Focus()
        {
            if (!IsFocused)
            {
                Logger.Debug("[{0}] :: Focus", this);
                Win32Helper.ForceForegroundWindow(_handle);
                WindowFocused?.Invoke(this);
            }
        }

        public void Hide()
        {
            Logger.Trace("[{0}] :: Hide", this);
            if (CanLayout)
            {
                _didManualHide = true;
            }
            Win32.ShowWindow(_handle, Win32.SW.SW_HIDE);
        }

        public void ShowNormal()
        {
            _didManualHide = false;
            Logger.Trace("[{0}] :: ShowNormal", this);
            Win32.ShowWindow(_handle, Win32.SW.SW_SHOWNOACTIVATE);
        }

        public void ShowMaximized()
        {
            _didManualHide = false;
            Logger.Trace("[{0}] :: ShowMaximized", this);
            Win32.ShowWindow(_handle, Win32.SW.SW_SHOWMAXIMIZED);
        }

        public void ShowMinimized()
        {
            _didManualHide = false;
            Logger.Trace("[{0}] :: ShowMinimized", this);
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

            WindowUpdated?.Invoke(this);
        }

        public void BringToTop()
        {
            Win32.BringWindowToTop(_handle);
            WindowUpdated?.Invoke(this);
        }

        public void Close()
        {
            Logger.Debug("[{0}] :: Close", this);
            Win32Helper.QuitApplication(_handle);
            WindowClosed?.Invoke(this);
        }

        public void NotifyUpdated()
        {
            WindowUpdated?.Invoke(this);
        }

        public override string ToString()
        {
            return $"[{Handle}][{Title}][{Class}][{ProcessName}]";
        }
    }
}
