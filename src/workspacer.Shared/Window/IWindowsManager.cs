namespace workspacer
{
    public delegate void WindowFocusDelegate(IWindow window);

    public interface IWindowsManager
    {
        IWindowsDeferPosHandle DeferWindowsPos(int count);
        void DumpWindowDebugOutput();
        void DumpWindowDebugOutputForFocusedWindow();
        void DumpWindowUnderCursorDebugOutput();

        event WindowFocusDelegate WindowFocused;

        void ToggleFocusedWindowTiling(); // mod-t
    }
}
