using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IConfigContext
    {
        IKeybindManager Keybinds { get; }
        ILayoutManager Layouts { get; }
        IWorkspaceManager Workspaces { get; }
        IPluginManager Plugins { get; }

        bool Enabled { get; set; }
        void Quit();
        void Restart();
    }
}
