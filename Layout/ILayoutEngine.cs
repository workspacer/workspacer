using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public interface ILayoutEngine
    {
        void DoLayout(IWorkspace workspace);
    }
}
