using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class VertLayoutEngine : PaneLayoutEngine
    {
        public VertLayoutEngine(int numInMaster) : base(true, numInMaster) { }
        public override string Name => "vert";
    }

    public class HorzLayoutEngine : PaneLayoutEngine
    {
        public HorzLayoutEngine(int numInMaster) : base(false, numInMaster) { }
        public override string Name => "horz";
    }

    public abstract class PaneLayoutEngine : ILayoutEngine
    {
        private readonly bool _vertical;
        private readonly int _numInMaster; 
        private int _numInMasterOffset = 0;

        public abstract string Name { get; }

        public PaneLayoutEngine(bool vertical, int numInMaster)
        {
            _vertical = vertical;
            _numInMaster = numInMaster;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            int numInMaster = Math.Min(GetNumInMaster(), numWindows);

            if (_vertical)
            {
                int width = (spaceWidth / numInMaster);
                int height = spaceHeight;

                for (var i = 0; i < numWindows; i++)
                {
                    if (i < numInMaster)
                        list.Add(new WindowLocation(i * width, 0, width, height, WindowState.Normal));
                    else
                        list.Add(new WindowLocation(0, 0, width, height, WindowState.Minimized));
                }
            } else
            {
                int height = (spaceHeight / numInMaster);
                int width = spaceWidth;

                for (var i = 0; i < numWindows; i++)
                {
                    if (i < numInMaster)
                        list.Add(new WindowLocation(0, i * height, width, height, WindowState.Normal));
                    else
                        list.Add(new WindowLocation(0, 0, width, height, WindowState.Minimized));
                }
            }

            return list;
        }

        public void DecrementNumInMaster()
        {
            if (GetNumInMaster() > 1)
            {
                _numInMasterOffset--;
            }
        }

        public void ExpandMasterArea()
        {
        }

        public void IncrementNumInMaster()
        {
            _numInMasterOffset++;
        }

        public void ResetMasterArea()
        {
            _numInMasterOffset = 0;
        }

        public void ShrinkMasterArea()
        {
        }

        private int GetNumInMaster()
        {
            return _numInMaster + _numInMasterOffset;
        }
    }
}
