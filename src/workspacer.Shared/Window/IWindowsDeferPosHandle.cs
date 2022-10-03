using System;

namespace workspacer
{
    public interface IWindowsDeferPosHandle : IDisposable
    {
        void DeferWindowPos(IWindow window, IWindowLocation location);
    }
}
