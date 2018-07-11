using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IWindowLocation
    {
        int X { get; }
        int Y { get; }
        int Width { get; }
        int Height { get; }
    }
}
