using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class VertLayoutEngine : PaneLayoutEngine
    {
        public VertLayoutEngine() : base(true) { }
        public override string Name => "vert";
    }

    public class HorzLayoutEngine : PaneLayoutEngine
    {
        public HorzLayoutEngine() : base(false) { }
        public override string Name => "horz";
    }

    public abstract class PaneLayoutEngine : ILayoutEngine
    {
        private readonly bool _vertical;

        public abstract string Name { get; }

        public PaneLayoutEngine(bool vertical)
        {
            _vertical = vertical;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            if (_vertical)
            {
                int width = (spaceWidth / numWindows);
                int height = spaceHeight;

                for (var i = 0; i < numWindows; i++)
                {
                    list.Add(new WindowLocation(i * width, 0, width, height, WindowState.Normal));
                }
            } else
            {
                int height = (spaceHeight / numWindows);
                int width = spaceWidth;

                for (var i = 0; i < numWindows; i++)
                {
                    list.Add(new WindowLocation(0, i * height, width, height, WindowState.Normal));
                }
            }

            return list;
        }

        public void DecrementNumInMaster() { }
        public void ExpandMasterArea() { }
        public void IncrementNumInMaster() { }
        public void ResetMasterArea() { }
        public void ShrinkMasterArea() { } 
    }
}
