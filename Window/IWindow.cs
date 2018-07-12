using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IWindow
    {
        IntPtr Handle { get; }
        string Title { get; }
        IWindowLocation Location { get; }

        bool CanLayout { get; }

        bool IsFocused { get; set; }
        bool IsMinimized { get; }
        bool IsMaximized { get; }
        bool CanResize { get; set; }

        void ShowNormal();
        void ShowMaximized();
        void ShowMinimized();
    }
}
