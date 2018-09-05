using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class FullLayoutEngine : ILayoutEngine
    {
        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            list.Add(new WindowLocation(0, 0, spaceWidth, spaceHeight, WindowState.Normal));

            for (var i = 1; i < numWindows; i++)
            {
                list.Add(new WindowLocation(0, 0, spaceWidth, spaceHeight, WindowState.Minimized));
            }
            return list;
        }

        public string Name => "full";

        public void ShrinkMasterArea() { }
        public void ExpandMasterArea() { }
        public void ResetMasterArea() { }
        public void IncrementNumInMaster() { }
        public void DecrementNumInMaster() { }
    }
}
