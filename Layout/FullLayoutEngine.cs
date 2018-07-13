using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Layout
{
    public class FullLayoutEngine : ILayoutEngine
    {
        public IEnumerable<IWindowLocation> CalcLayout(int numWindows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();

            if (numWindows == 0)
                return list;

            list.Add(new WindowLocation(0, 0, spaceWidth, spaceHeight, WindowState.Maximized));

            for (var i = 1; i < numWindows; i++)
            {
                list.Add(new WindowLocation(0, 0, spaceWidth, spaceHeight, WindowState.Maximized));
            }
            return list;
        }

        public void ShrinkMasterArea() { }
        public void ExpandMasterArea() { }
        public void ResetMasterArea() { }
        public void IncrementNumInMaster() { }
        public void DecrementNumInMaster() { }
    }
}
