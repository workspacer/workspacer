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
            int rows;
            int cols;

            if (numWindows == 0)
            {
                return list;
            }
            else if (numWindows < 4)
            {
                cols = numWindows;
                rows = 1;
            }
            else
            {
                cols = numWindows % 2 == 0 ? numWindows / 2 : (numWindows + 1) / 2;
                rows = 2;
            }


            for (int i = 0; i < numWindows; i++)
            {

                if (i < cols)
                {
                    list.Add(new WindowLocation(spaceWidth / cols * i, 0, spaceWidth / cols, spaceHeight / rows, WindowState.Normal));
                }
                else
                {
                    list.Add(new WindowLocation(spaceWidth / cols * (i - cols), spaceHeight / rows, spaceWidth / cols, spaceHeight / rows, WindowState.Normal));
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
