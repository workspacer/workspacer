using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.ConfigLoader
{
    public interface IWorkspaceManager
    {
        Func<IWindow, IWorkspace> WorkspaceSelectorFunc { get; set; }
        Func<IWindow, bool> WindowFilterFunc { get; set; }
        void AddWorkspace(string name, ILayoutEngine[] layouts);
        IWorkspace FocusedWorkspace { get; }
        void SwitchToWorkspace(int index);
        void MoveFocusedWindowToWorkspace(int index);
    }
}
