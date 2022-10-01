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
                if (handle == IntPtr.Zero)
                    return false;

                return Win32.GetWindowStyleLongPtr(handle).HasFlag(Win32.WS.WS_VISIBLE);
            }
            set
            {
                var handle = Win32.GetConsoleWindow();
                if (handle == IntPtr.Zero)
                    return;

                Win32.ShowWindow(handle, value ? Win32.SW.SW_SHOW : Win32.SW.SW_HIDE);
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
