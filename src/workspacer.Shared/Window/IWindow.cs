using System;
using System.Drawing;

namespace workspacer
{
    public delegate void IWindowDelegate(IWindow sender);

    public interface IWindow
    {
        /// <summary>
        /// Notifies when the Close function was called on the window
        /// </summary>
        event IWindowDelegate WindowClosed;
        /// <summary>
        /// Notifies that an update action was called on the window
        /// This includes SetUpdated, all Show* functions and BringToTop
        /// </summary>
        event IWindowDelegate WindowUpdated;
        /// <summary>
        /// Notifies that the Focus function was called on the window
        /// </summary>
        event IWindowDelegate WindowFocused;

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

        /// <summary>
        /// Notifies subscribers of the WindowUpdated event
        /// This is used when code outside this class modifies the window state
        /// E.g. swapping windows in a workspace
        /// </summary>
        void SetUpdated();
    }
}
