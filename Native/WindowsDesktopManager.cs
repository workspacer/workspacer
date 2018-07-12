using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tile.Net.Native;

namespace Tile.Net
{
    public delegate void WindowDelegate(IWindow window);

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

        private WinEventDelegate _hookDelegate;

        public event WindowDelegate WindowCreated;
        public event WindowDelegate WindowDestroyed;
        public event WindowDelegate WindowUpdated;

        public IEnumerable<IWindow> Windows => _windows.Values;

        private WindowsDesktopManager()
        {
            _windows = new Dictionary<IntPtr, IWindow>();
            _pollTimers = new Dictionary<IntPtr, Timer>();

            _hookDelegate = new WinEventDelegate(WindowHook);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_OBJECT_DESTROY, Win32.EVENT_CONSTANTS.EVENT_OBJECT_SHOW, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_OBJECT_CLOAKED, Win32.EVENT_CONSTANTS.EVENT_OBJECT_UNCLOAKED, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZESTART, Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, _hookDelegate, 0, 0, 0);

            Win32.EnumWindows((handle, param) =>
            {
                if (Win32Helper.IsAppWindow(handle))
                {
                    RegisterWindow(handle);
                }
                return true;
            }, IntPtr.Zero);
        }

        public WindowsDeferPosHandle DeferWindowsPos(int count)
        {
            var info = Win32.BeginDeferWindowPos(count);
            return new WindowsDeferPosHandle(info);
        }

        private static WindowsDesktopManager CreateInstance()
        {
            return new WindowsDesktopManager();
        }
        private void WindowHook(IntPtr hWinEventHook, Win32.EVENT_CONSTANTS eventType, IntPtr hwnd, Win32.OBJID idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (EventWindowIsValid(idChild, idObject, hwnd))
            {
                switch (eventType)
                {
                    case Win32.EVENT_CONSTANTS.EVENT_OBJECT_SHOW:
                        RegisterWindow(hwnd);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_OBJECT_DESTROY:
                        UnregisterWindow(hwnd);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_OBJECT_CLOAKED:
                    case Win32.EVENT_CONSTANTS.EVENT_OBJECT_UNCLOAKED:
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZESTART:
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZEEND:
                        UpdateWindow(hwnd);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MOVESIZESTART:
                        StartWindowMove(hwnd);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MOVESIZEEND:
                        EndWindowMove(hwnd);
                        break;
                }
            }
        }

        private bool EventWindowIsValid(int idChild, Win32.OBJID idObject, IntPtr hwnd)
        {
            return idChild == Win32.CHILDID_SELF && idObject == Win32.OBJID.OBJID_WINDOW && hwnd != IntPtr.Zero;
        }
        
        private void RegisterWindow(IntPtr handle)
        {
            if (!_windows.ContainsKey(handle))
            {
                var window = new WindowsWindow(handle);
                _windows[handle] = window;
                WindowCreated?.Invoke(window);
            }
        }

        private void UnregisterWindow(IntPtr handle)
        {
            if (_windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                _windows.Remove(handle);
                WindowDestroyed?.Invoke(window);
            }
        }

        private void UpdateWindow(IntPtr handle)
        {
            if (_windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                WindowUpdated?.Invoke(window);
            }
        }

        private void StartWindowMove(IntPtr handle)
        {
            if (_pollTimers.ContainsKey(handle))
            {
                _pollTimers[handle].Stop();
            }
            _pollTimers[handle] = CreatePollTimer(handle);
            _pollTimers[handle].Start();
        }

        private void EndWindowMove(IntPtr handle)
        {
            if (_pollTimers.ContainsKey(handle))
            {
                _pollTimers[handle].Stop();
                _pollTimers.Remove(handle);
            }
        }

        private Timer CreatePollTimer(IntPtr handle)
        {
            var timer = new Timer(10);
            timer.AutoReset = true;
            timer.Elapsed += (sender, e) =>
            {
                if (_windows.ContainsKey(handle))
                {
                    var window = _windows[handle];
                }
            };
            return timer;
        }
    }
}
