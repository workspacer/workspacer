using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface IWindow
    {
        string Title { get; }
        IWindowLocation Location { get; }

        bool CanLayout { get; }

        bool CanResize { get; set; }
    }
}
