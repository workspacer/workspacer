using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.ConfigLoader
{
    public interface IConfigContext
    {
        IKeybindManager Keybinds { get; }
        ILayoutManager Layouts { get; }
        IWorkspaceManager Workspaces { get; }

        void Quit();
        void Restart();
    }
}
