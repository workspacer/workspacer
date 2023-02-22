using System;

namespace workspacer
{
    public interface ISystemTrayManager : IDisposable
    {
        void AddToContextMenu(string text, Action handler);
    }
}
