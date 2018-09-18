using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IMonitor
    {
        string Name { get; }
        int Index { get; }
        int Width { get; }
        int Height { get; }
        int X { get; }
        int Y { get; }
        
        IWorkspace Workspace { get; set; }
    }
}
