using System;

namespace workspacer
{
    public static class FocusStealer
    {
        private static Logger Logger = Logger.Create();

        public static void Steal(IntPtr windowToFocus)
        {
            Win32.keybd_event(0, 0, 0, UIntPtr.Zero);

            Win32.SetForegroundWindow(windowToFocus);
        }
    }
}
