using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace Workspacer
{
    public static class FocusStealer
    {
        private delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);
        private delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

        private static volatile MessageWindow _wnd;
        private static volatile IntPtr _hwnd;
        private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);
        private static int _id = 0;

        private static Keys _key = Keys.F12;
        private static uint _modifiers = 0x1 | 0x2 | 0x4;

        private static IntPtr _windowToFocus = IntPtr.Zero;
        private static InputSimulator _inputSim = new InputSimulator();

        public static void Initialize()
        {
            var window = new MessageWindow();
            
            _windowReadyEvent.WaitOne();
            int id = Interlocked.Increment(ref _id);
            _wnd.Invoke(new RegisterHotKeyDelegate(RegisterHotKeyInternal), _hwnd, id, (uint)_modifiers, (uint)_key);
            
            Application.Run(window);
        }

        public static void Steal(IntPtr windowToFocus)
        {
            _windowToFocus = windowToFocus;
            _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.SHIFT);
            _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
            _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            _inputSim.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F12);
            _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.SHIFT);
            _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);
            _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);
        }

        private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
        {
            var success = Win32.RegisterHotKey(hwnd, id, modifiers, key);
            var error = Marshal.GetLastWin32Error();
        }

        private class MessageWindow : Form
        {
            public MessageWindow()
            {
                _wnd = this;
                _hwnd = this.Handle;
                _windowReadyEvent.Set();
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    if (_windowToFocus != IntPtr.Zero)
                    {
                        var success = Win32.SetForegroundWindow(_windowToFocus);
                        _windowToFocus = IntPtr.Zero;
                    }
                }

                base.WndProc(ref m);
            }

            protected override void SetVisibleCore(bool value)
            {
                // Ensure the window never becomes visible
                base.SetVisibleCore(false);
            }

            private const int WM_HOTKEY = 0x312;
        }
    }
}
