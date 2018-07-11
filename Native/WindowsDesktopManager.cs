using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Tile.Net
{
    public class WindowsDesktopManager
    {

        private static WindowsDesktopManager _instance;
        public static WindowsDesktopManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = CreateInstance();
                return _instance;
            }
        }

        private IDictionary<IntPtr, IWindow> _windows;
        private IDictionary<IntPtr, Timer> _pollTimers;

        private WinEventDelegate _moveDelegate;
        private WinEventDelegate _showDelegate;
        private WinEventDelegate _hideDelegate;
        private WinEventDelegate _destroyDelegate;

        private WindowsDesktopManager()
        {
            _windows = new Dictionary<IntPtr, IWindow>();
            _pollTimers = new Dictionary<IntPtr, Timer>();

            _moveDelegate = new WinEventDelegate(WindowMoved);
            Win32.SetWinEventHook(Win32.EVENT_SYSTEM_MOVESIZESTART, Win32.EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, _moveDelegate, 0, 0, 0);

            _showDelegate = new WinEventDelegate(WindowShowed);
            Win32.SetWinEventHook(Win32.EVENT_OBJECT_SHOW, Win32.EVENT_OBJECT_SHOW, IntPtr.Zero, _showDelegate, 0, 0, 0);

            _hideDelegate = new WinEventDelegate(WindowHidden);
            Win32.SetWinEventHook(Win32.EVENT_OBJECT_HIDE, Win32.EVENT_OBJECT_HIDE, IntPtr.Zero, _hideDelegate, 0, 0, 0);

            _destroyDelegate = new WinEventDelegate(WindowDestroyed);
            Win32.SetWinEventHook(Win32.EVENT_OBJECT_DESTROY, Win32.EVENT_OBJECT_DESTROY, IntPtr.Zero, _destroyDelegate, 0, 0, 0);

            Win32.EnumDesktopWindows(IntPtr.Zero, (handle, param) =>
            {
                if (Win32.IsWindowVisible(handle))
                {
                    RegisterWindow(handle);
                }
                return true;
            }, IntPtr.Zero);
        }

        private static WindowsDesktopManager CreateInstance()
        {
            return new WindowsDesktopManager();
        }
        private void WindowShowed(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, Win32.OBJID idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (EventWindowIsValid(idChild, idObject, hwnd) && IsAppWindow(hwnd))
            {
                RegisterWindow(hwnd);
            }
        }
        private void WindowHidden(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, Win32.OBJID idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //UnregisterWindow(hwnd);
        }
        private void WindowDestroyed(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, Win32.OBJID idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            UnregisterWindow(hwnd);
        }

        private void WindowMoved(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, Win32.OBJID idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType == Win32.EVENT_SYSTEM_MOVESIZESTART)
            {
                if (_pollTimers.ContainsKey(hwnd))
                {
                    _pollTimers[hwnd].Stop();
                }
                _pollTimers[hwnd] = CreatePollTimer(hwnd);
                _pollTimers[hwnd].Start();
            }
            else if (eventType == Win32.EVENT_SYSTEM_MOVESIZEEND)
            {
                if (_pollTimers.ContainsKey(hwnd))
                {
                    _pollTimers[hwnd].Stop();
                    _pollTimers.Remove(hwnd);
                }
            }
        }

        private bool EventWindowIsValid(int idChild, Win32.OBJID idObject, IntPtr hwnd)
        {
            return idChild == Win32.CHILDID_SELF && idObject == Win32.OBJID.OBJID_WINDOW && hwnd != IntPtr.Zero;
        }

        private bool IsAppWindow(IntPtr hwnd)
        {
            return Win32.IsWindowVisible(hwnd) &&
                   Win32.GetWindowExStyleLongPtr(hwnd).HasFlag(Win32.WS_EX.WS_EX_NOACTIVATE) &&
                   Win32.GetWindowStyleLongPtr(hwnd).HasFlag(Win32.WS.WS_CHILD);
        }

        private void RegisterWindow(IntPtr handle)
        {
            if (!_windows.ContainsKey(handle))
            {
                var window = new WindowsWindow(handle);
                _windows[handle] = window;
                Console.WriteLine($"[{window.Title}] registered");
            }
        }

        private void UnregisterWindow(IntPtr handle)
        {
            if (_windows.ContainsKey(handle))
            {
                Console.WriteLine($"[{_windows[handle].Title}] unregistered");
                _windows.Remove(handle);
            }
        }

        private Timer CreatePollTimer(IntPtr handle)
        {
            var timer = new Timer(10);
            timer.AutoReset = true;
            timer.Elapsed += (sender, e) =>
            {
                var window = _windows[handle];
                // do event for moving
            };
            return timer;
        }
    }
}
