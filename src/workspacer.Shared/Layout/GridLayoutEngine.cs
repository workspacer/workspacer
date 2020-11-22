using System;
using System.Collections.Generic;
using System.Linq;

namespace workspacer
{
    public class GridLayoutEngine : ILayoutEngine
    {
        public GridLayoutEngine() { }

        public string Name => "grid";

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            int rows = (int)(Math.Pow(numWindows, 0.5) + 0.5);

            while (numWindows % rows != 0)
            {
                rows -= 1;
            }

            int cols = numWindows / rows;
            int tileWidth = spaceWidth / cols;
            int tileHeight = spaceHeight / rows;


            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    list.Add(new WindowLocation(tileWidth * c, tileHeight * r, tileWidth, tileHeight, WindowState.Normal));
                }
            }

            return list;
        }

        public void ShrinkPrimaryArea() { }
        public void ExpandPrimaryArea() { }
        public void ResetPrimaryArea() { }
        public void IncrementNumInPrimary() { }
        public void DecrementNumInPrimary() { }

    }
}
