using System;
using System.Drawing;

namespace workspacer
{
    public delegate void WindowUpdatedDelegate(WindowUpdateType type);
    public delegate void WindowClosedDelegate();
    public delegate void WindowFocusedDelegate();

    public interface IWindow
    {
        event WindowClosedDelegate WindowClosed;
        event WindowUpdatedDelegate WindowUpdated;
        event WindowFocusedDelegate WindowFocused;

        IntPtr Handle { get; }
        string Title { get; }
        string Class { get; }
        IWindowLocation Location { get; }
        Rectangle Offset { get; }

        int ProcessId { get; }
        string ProcessFileName { get; }
        string ProcessName { get; }

        bool CanLayout { get; }

        bool IsFocused { get; }
        bool IsMinimized { get; }
        bool IsMaximized { get; }
        bool IsMouseMoving { get; }

        void Focus();
        void Hide();
        void ShowNormal();
        void ShowMaximized();
        void ShowMinimized();
        void ShowInCurrentState();

        void BringToTop();

        void Close();

        void Updated(WindowUpdateType type);
    }
}
