using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.ConfigLoader
{
    public interface ILayoutManager
    {
        void AddLayout(Func<ILayoutEngine> func);
        ILayoutEngine[] CreateLayouts();
    }
}
