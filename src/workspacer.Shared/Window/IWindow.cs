using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface IWindow
    {
        IntPtr Handle { get; }
        string Title { get; }
        string Class { get; }
        IWindowLocation Location { get; }

        Process Process { get; }
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
    }
}
