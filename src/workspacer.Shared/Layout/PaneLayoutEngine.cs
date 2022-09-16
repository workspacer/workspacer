using System;
using System.Collections.Generic;
using System.Linq;

namespace workspacer
{
    public class VertLayoutEngine : PaneLayoutEngine
    {
        public VertLayoutEngine() : base(true) { }
        public override string Name => "rows";
    }

    public class HorzLayoutEngine : PaneLayoutEngine
    {
        public HorzLayoutEngine() : base(false) { }
        public override string Name => "columns";
    }

    public abstract class PaneLayoutEngine : ILayoutEngine
    {
        private readonly bool _vertical;

        private readonly int _numInPrimary;

        private readonly double _primaryPercentIncrement;

        private int _numInPrimaryOffset = 0;

        private double _primaryPercentOffset = 0;

        public abstract string Name { get; }

        public PaneLayoutEngine(bool vertical) : this(vertical, 1, 0.03) { }

        public PaneLayoutEngine(bool vertical, int numInPrimary, double primaryPercentIncrement)
        {
            _vertical = vertical;
            _numInPrimary = numInPrimary;
            _primaryPercentIncrement = primaryPercentIncrement;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            double primaryPercent = (1.0D / numWindows) + _primaryPercentOffset;
            int numInPrimary = Math.Min(GetNumInPrimary(), numWindows);
            int numInSecondary = numWindows - numInPrimary;

            if (_vertical)
            {
                int width = spaceWidth;

                int height = 0;
                int primaryHeight = 0;

                if (numInSecondary == 0)
                {
                    // All the windows are in the primary area
                    primaryHeight = spaceHeight / numWindows;
                }
                else
                {
                    primaryHeight = (int)(spaceHeight * primaryPercent / numInPrimary);
                    height = (spaceHeight - primaryHeight * numInPrimary) / numInSecondary;
                }

                int startY = 0;

                for (var i = 0; i < numWindows; i++)
                {
                    int currentHeight = i < numInPrimary ? primaryHeight : height;

                    list.Add(new WindowLocation(0, startY, width, currentHeight, WindowState.Normal));
                    startY += currentHeight;
                }
            }
            else
            {
                int width = 0;
                int primaryWidth = 0;

                if (numInSecondary == 0)
                {
                    // All the windows are in the primary area
                    primaryWidth = spaceWidth / numWindows;
                }
                else
                {
                    primaryWidth = (int)(spaceWidth * primaryPercent / numInPrimary);
                    width = (spaceWidth - primaryWidth * numInPrimary) / numInSecondary;
                }

                int height = spaceHeight;
                int startX = 0;

                for (var i = 0; i < numWindows; i++)
                {
                    int currentWidth = i < numInPrimary ? primaryWidth : width;

                    list.Add(new WindowLocation(startX, 0, currentWidth, height, WindowState.Normal));
                    startX += currentWidth;
                }
            }

            return list;
        }

        public void IncrementNumInPrimary()
        {
            _numInPrimaryOffset++;
        }

        public void DecrementNumInPrimary()
        {
            if (GetNumInPrimary() > 1)
            {
                _numInPrimaryOffset--;
            }
        }

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

        private int GetNumInPrimary()
        {
            return _numInPrimary + _numInPrimaryOffset;
        }
    }
}
