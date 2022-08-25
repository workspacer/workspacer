using System;
using System.IO;

namespace workspacer
{
    public static class ConsoleHelper
    {
        public static void Initialize()
        {
            Win32.AllocConsole();
            var handle = Win32.GetConsoleWindow();
            Win32.SetWindowPos(handle, Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.SetWindowPosFlags.IgnoreMove | Win32.SetWindowPosFlags.IgnoreResize);

            Win32.DeleteMenu(Win32.GetSystemMenu(Win32.GetConsoleWindow(), false), Win32.SC_CLOSE, Win32.MF_BYCOMMAND);

            Console.Title = "workspacer debug";

            OverrideRedirection();
            IsConsoleShowing = false;
        }
        public static bool IsConsoleShowing
        {
            get
            {

                var handle = Win32.GetConsoleWindow();
                if (handle != IntPtr.Zero)
                {
                    var style = Win32.GetWindowLongPtr(handle, Win32.GWL_STYLE);
                    return ((uint)style & (uint)Win32.WS.WS_VISIBLE) != 0;
                }
                return false;
            }
            set
            {
                var handle = Win32.GetConsoleWindow();
                if (handle == IntPtr.Zero)
                    return;

                if (IsConsoleShowing)
                {
                    Win32.ShowWindow(handle, Win32.SW.SW_HIDE);
                }
                else
                {
                    Win32.ShowWindow(handle, Win32.SW.SW_SHOW);
                }
            }
        }

        public static void ToggleConsoleWindow()
        {
            IsConsoleShowing = !IsConsoleShowing;
        }

        private static void OverrideRedirection()
        {
            var hOut = Win32.GetStdHandle(Win32.STD_OUTPUT_HANDLE);
            var hRealOut = Win32.CreateFile("CONOUT$", Win32.GENERIC_READ | Win32.GENERIC_WRITE, FileShare.Write, IntPtr.Zero, FileMode.OpenOrCreate, 0, IntPtr.Zero);
            Win32.SetStdHandle(Win32.STD_OUTPUT_HANDLE, hRealOut);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) { AutoFlush = true });
        }
    }
}
