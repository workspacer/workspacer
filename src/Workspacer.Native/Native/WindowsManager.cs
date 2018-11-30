﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Workspacer
{
    public delegate void WindowDelegate(IWindow window);
    public delegate void WindowUpdateDelegate(IWindow window, WindowUpdateType type);

    public class WindowsManager : IWindowsManager
    {
        private Logger Logger = Logger.Create();

        private IDictionary<IntPtr, WindowsWindow> _windows;
        private WinEventDelegate _hookDelegate;

        private WindowsWindow _mouseMoveWindow;
        private readonly object _mouseMoveLock = new object();
        private Win32.HookProc _mouseHook;

        private Dictionary<WindowsWindow, bool> _floating;

        public event WindowDelegate WindowCreated;
        public event WindowDelegate WindowDestroyed;
        public event WindowUpdateDelegate WindowUpdated;
        public event WindowFocusDelegate WindowFocused;

        public IEnumerable<IWindow> Windows => _windows.Values;

        public WindowsManager()
        {
            _windows = new Dictionary<IntPtr, WindowsWindow>();
            _floating = new Dictionary<WindowsWindow, bool>();
            _hookDelegate = new WinEventDelegate(WindowHook);
        }

        public void Initialize()
        {
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_OBJECT_DESTROY, Win32.EVENT_CONSTANTS.EVENT_OBJECT_SHOW, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_OBJECT_CLOAKED, Win32.EVENT_CONSTANTS.EVENT_OBJECT_UNCLOAKED, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZESTART, Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MOVESIZESTART, Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_SYSTEM_FOREGROUND, Win32.EVENT_CONSTANTS.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_OBJECT_NAMECHANGE, Win32.EVENT_CONSTANTS.EVENT_OBJECT_NAMECHANGE, IntPtr.Zero, _hookDelegate, 0, 0, 0);
            Win32.SetWinEventHook(Win32.EVENT_CONSTANTS.EVENT_OBJECT_LOCATIONCHANGE, Win32.EVENT_CONSTANTS.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, _hookDelegate, 0, 0, 0);

            _mouseHook = MouseHook;

            Win32.EnumWindows((handle, param) =>
            {
                if (Win32Helper.IsAppWindow(handle))
                {
                    var window = RegisterWindow(handle, false);
                }
                return true;
            }, IntPtr.Zero);

            var thread = new Thread(() =>
            {
                Win32.SetWindowsHookEx(Win32.WH_MOUSE_LL, _mouseHook, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
                Application.Run();
            });
            thread.Name = "WindowsManager";
            thread.Start();
        }

        public IWindowsDeferPosHandle DeferWindowsPos(int count)
        {
            var info = Win32.BeginDeferWindowPos(count);
            return new WindowsDeferPosHandle(info);
        }

        public void DumpWindowDebugOutput()
        {
            var output = "";
            foreach (var window in Windows)
            {
                output += GenerateWindowDebugOutput(window) + "\r\n\r\n";
            }
            OpenDebugOutput(output);
        }

        public void DumpWindowUnderCursorDebugOutput()
        {
            var location = Control.MousePosition;
            var handle = Win32.WindowFromPoint(location);
            if (_windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                var output = GenerateWindowDebugOutput(window);
                OpenDebugOutput(output);
            }
        }

        public void ToggleFocusedWindowTiling()
        {
            var window = _windows.Values.FirstOrDefault(w => w.IsFocused);

            if (window != null)
            {
                if (_floating.ContainsKey(window))
                {
                    _floating.Remove(window);
                    HandleWindowAdd(window);
                }
                else
                {
                    _floating[window] = true;
                    HandleWindowRemove(window);
                    window.BringToTop();
                }
                window.Focus();
            }
        }

        private IntPtr MouseHook(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode == 0 && (uint)wParam == Win32.WM_LBUTTONUP) {
                Logger.Debug("MouseHook - WM_LBUTTONUP");
                HandleWindowMoveEnd();
            }
            return Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private void OpenDebugOutput(string output)
        {
            Logger.Debug(output);
        }

        private string GenerateWindowDebugOutput(IWindow window)
        {
            var builder = new StringBuilder();

            builder.AppendLine("#############################");
            builder.AppendLine($"window.Handle                    = {window.Handle}");
            builder.AppendLine($"window.Title                     = \"{window.Title}\"");
            builder.AppendLine($"window.Class                     = \"{window.Class}\"");
            builder.AppendLine($"window.Process.Id                = {window.Process.Id}");
            builder.AppendLine($"window.Process.ProcessName       = \"{window.Process.ProcessName}\"");
            builder.AppendLine($"window.ProcessFileName           = \"{window.ProcessFileName}\"");
            builder.AppendLine("#############################");

            return builder.ToString();
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
                        UpdateWindow(hwnd, WindowUpdateType.Hide);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_OBJECT_UNCLOAKED:
                        UpdateWindow(hwnd, WindowUpdateType.Show);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZESTART:
                        UpdateWindow(hwnd, WindowUpdateType.MinimizeStart);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MINIMIZEEND:
                        UpdateWindow(hwnd, WindowUpdateType.MinimizeEnd);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_FOREGROUND:
                        UpdateWindow(hwnd, WindowUpdateType.Foreground);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_OBJECT_NAMECHANGE:
                        UpdateWindow(hwnd, WindowUpdateType.TitleChange);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MOVESIZESTART:
                        StartWindowMove(hwnd);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_SYSTEM_MOVESIZEEND:
                        EndWindowMove(hwnd);
                        break;
                    case Win32.EVENT_CONSTANTS.EVENT_OBJECT_LOCATIONCHANGE:
                        WindowMove(hwnd);
                        break;
                }
            }
        }

        private bool EventWindowIsValid(int idChild, Win32.OBJID idObject, IntPtr hwnd)
        {
            return idChild == Win32.CHILDID_SELF && idObject == Win32.OBJID.OBJID_WINDOW && hwnd != IntPtr.Zero;
        }
        
        private WindowsWindow RegisterWindow(IntPtr handle, bool emitEvent = true)
        {
            if (!_windows.ContainsKey(handle))
            {
                var window = new WindowsWindow(handle);
                window.WindowFocused += () => HandleWindowFocused(window);
                _windows[handle] = window;

                if (emitEvent)
                {
                    HandleWindowAdd(window);
                }

                return window;
            } else
            {
                return _windows[handle];
            }
        }

        private void UnregisterWindow(IntPtr handle)
        {
            if (_windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                _windows.Remove(handle);
                HandleWindowRemove(window);
            }
        }

        private void UpdateWindow(IntPtr handle, WindowUpdateType type)
        {
            if (type == WindowUpdateType.Show  && _windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                WindowUpdated?.Invoke(window, type);
            }
            else if (type == WindowUpdateType.Show)
            {
                RegisterWindow(handle);
            }
            else if (type == WindowUpdateType.Hide && _windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                if (!window.DidManualHide)
                {
                    UnregisterWindow(handle);
                } else
                {
                    WindowUpdated?.Invoke(window, type);
                }
            }
            else if (_windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                WindowUpdated?.Invoke(window, type);
            }
        }

        private void StartWindowMove(IntPtr handle)
        {
            if (_windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                Logger.Debug("StartWindowMove[{0}]", window);

                HandleWindowMoveStart(window);
                WindowUpdated?.Invoke(window, WindowUpdateType.MoveStart);
            }
        }

        private void EndWindowMove(IntPtr handle)
        {
            if (_windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                Logger.Debug("EndWindowMove[{0}]", window);

                HandleWindowMoveEnd();
                WindowUpdated?.Invoke(window, WindowUpdateType.MoveEnd);
            }
        }

        private void WindowMove(IntPtr handle)
        {
            if (_mouseMoveWindow != null && _windows.ContainsKey(handle))
            {
                var window = _windows[handle];
                if (_mouseMoveWindow == window)
                {
                    WindowUpdated?.Invoke(window, WindowUpdateType.Move);
                }
            }
        }

        private void HandleWindowFocused(WindowsWindow window)
        {
            WindowFocused?.Invoke(window);
        }

        private void HandleWindowMoveStart(WindowsWindow window)
        {
            if (_mouseMoveWindow != null)
            {
                _mouseMoveWindow.IsMouseMoving = false;
            }

            _mouseMoveWindow = window;
            window.IsMouseMoving = true;
        }

        private void HandleWindowMoveEnd()
        {
            lock (_mouseMoveLock)
            {
                if (_mouseMoveWindow != null)
                {
                    _mouseMoveWindow.IsMouseMoving = false;
                    _mouseMoveWindow = null;
                }
            }
        }

        private void HandleWindowAdd(WindowsWindow window)
        {
            WindowCreated?.Invoke(window);
        }

        private void HandleWindowRemove(WindowsWindow window)
        {
            WindowDestroyed?.Invoke(window);
        }
    }
}
