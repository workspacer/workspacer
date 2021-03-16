using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
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

        private int _primaryIdx;

        private readonly double _primaryPercentIncrement;

        private double _primaryPercentOffset = 0;

        public abstract string Name { get; }

        public PaneLayoutEngine(bool vertical) : this(vertical, 0, 0.03) { }

        public PaneLayoutEngine(bool vertical, int primaryIdx, double primaryPercentIncrement)
        {
            _vertical = vertical;
            _primaryIdx = primaryIdx;
            _primaryPercentIncrement = primaryPercentIncrement;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();
            _primaryIdx %= numWindows;

            if (numWindows == 0)
                return list;

            double currentPercent = (1.0D / numWindows) + _primaryPercentOffset;
            if (_vertical)
            {
                int primaryHeight = (int)(spaceHeight * currentPercent);
                int width = spaceWidth;
                int height = numWindows <= 1 ? spaceHeight : ((spaceHeight - primaryHeight) / (numWindows - 1));
                int startY = 0;

                for (var i = 0; i < numWindows; i++)
                {
                    int currentHeight = i == _primaryIdx ? primaryHeight : height;

                    list.Add(new WindowLocation(0, startY, width, currentHeight, WindowState.Normal));
                    startY += currentHeight;
                }
            }
            else
            {
                int primaryWidth = (int)(spaceWidth * currentPercent);
                int width = numWindows <= 1 ? spaceWidth : ((spaceWidth - primaryWidth) / (numWindows - 1));
                int height = spaceHeight;
                int startX = 0;

                for (var i = 0; i < numWindows; i++)
                {
                    int currentWidth = i == _primaryIdx ? primaryWidth : width;

                    list.Add(new WindowLocation(startX, 0, currentWidth, height, WindowState.Normal));
                    startX += currentWidth;
                }
            }

            return list;
        }

        public void DecrementNumInPrimary() { }
        public void IncrementNumInPrimary() { }
        public void ExpandPrimaryArea()
        {
            _primaryPercentOffset += _primaryPercentIncrement;
        }

        public void ResetPrimaryArea()
        {
            _primaryPercentOffset = 0;
        }
        public void ShrinkPrimaryArea()
        {
            _primaryPercentOffset -= _primaryPercentIncrement;
        }
    }
}
