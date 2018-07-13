using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class HorizontalLayoutEngine : ILayoutEngine
    {
        public IEnumerable<IWindowLocation> CalcLayout(int numWindows, int spaceWidth, int spaceHeight)
        {
            if (numWindows == 0)
                return Enumerable.Empty<IWindowLocation>();

            var windowWidth = spaceWidth / numWindows;
            var extra = spaceWidth % numWindows;

            var list = new List<IWindowLocation>();

            list.Add(new WindowLocation(0, 0, windowWidth + extra, spaceHeight, WindowState.Normal));

            int offset = windowWidth + extra;
            for (var i = 1; i < numWindows; i++)
            {
                list.Add(new WindowLocation(offset, 0, windowWidth, spaceHeight, WindowState.Normal));
                offset += windowWidth;
            }

            return list;
        }

        public void ShrinkMasterArea()
        {
        }

        public void ExpandMasterArea()
        {
        }

        public void ResetMasterArea()
        {
        }

        public void IncrementNumInMaster()
        {
        }

        public void DecrementNumInMaster()
        {
        }
    }
}
