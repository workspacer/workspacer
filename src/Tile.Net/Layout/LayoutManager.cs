using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class LayoutManager
    {
        public static LayoutManager Instance { get; } = new LayoutManager();

        private List<Func<ILayoutEngine>> _layoutFuncs;

        private LayoutManager()
        {
            _layoutFuncs = new List<Func<ILayoutEngine>>();
        }

        public void AddLayout(Func<ILayoutEngine> func)
        {
            _layoutFuncs.Add(func);
        }

        public ILayoutEngine[] CreateLayouts()
        {
            return _layoutFuncs.Select(f => f()).ToArray();
        }
    }
}
