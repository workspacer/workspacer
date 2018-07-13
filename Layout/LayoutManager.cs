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
        private sealed class LayoutDef
        {
            public Type Type { get; set; }
            public object[] Params { get; set; }
        }

        public static LayoutManager Instance { get; } = new LayoutManager();

        private List<LayoutDef> _layoutDefs;

        private LayoutManager()
        {
            _layoutDefs = new List<LayoutDef>();
        }

        public void AddLayout<T>(params object[] args) where T : ILayoutEngine
        {
            _layoutDefs.Add(new LayoutDef()
            {
                Type = typeof(T),
                Params = args,
            });
        }

        public ILayoutEngine[] CreateLayouts()
        {
            return _layoutDefs
                .Select(d => Activator.CreateInstance(d.Type, d.Params))
                .Cast<ILayoutEngine>().ToArray();
        }
    }
}
