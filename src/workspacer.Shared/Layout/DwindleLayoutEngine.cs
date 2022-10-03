using System;
using System.Collections.Generic;
using System.Linq;

namespace workspacer
{
    public static class Orientation
    {
        public const bool Horizontal = true;
        public const bool Vertical = false;
    }

    public class DwindleLayoutEngine : ILayoutEngine
    {
        private readonly int _numInPrimary;
        private readonly double _primaryPercent;
        private readonly double _primaryPercentIncrement;

        private int _numInPrimaryOffset = 0;
        private double _primaryPercentOffset = 0;

        public DwindleLayoutEngine() : this(1, 0.5, 0.03) { }

        public DwindleLayoutEngine(int numInPrimary, double primaryPercent, double primaryPercentIncrement)
        {
            _numInPrimary = numInPrimary;
            _primaryPercent = primaryPercent;
            _primaryPercentIncrement = primaryPercentIncrement;
        }

        public string Name { get; set; } = "dwindle";


        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            int numInPrimary = Math.Min(GetNumInPrimary(), numWindows);

            int primaryWidth = (int)(spaceWidth * (_primaryPercent + _primaryPercentOffset));
            int primaryHeight = spaceHeight / numInPrimary;
            int height = spaceHeight / Math.Max(numWindows - numInPrimary, 1);

            // if there are more "primary" windows than actual windows,
            // then we want the pane to actually spread the entire width
            // of the working area
            if (numInPrimary >= numWindows)
            {
                primaryWidth = spaceWidth;
            }

            int secondaryWidth = spaceWidth - primaryWidth;

            // Recurse in a 'dwindle fibonacci' pattern
            bool curOrientation = Orientation.Vertical;
            int curWidth = secondaryWidth;
            int curTop = 0;
            int curLeft = primaryWidth;
            int curHeight = numWindows > 2 ? spaceHeight / 2 : spaceHeight;
            for (var i = 0; i < numWindows; i++)
            {
                if (i < numInPrimary)
                {
                    list.Add(new WindowLocation(0, i * primaryHeight, primaryWidth, primaryHeight, WindowState.Normal));
                }
                else
                {
                    list.Add(new WindowLocation(curLeft, curTop, curWidth, curHeight, WindowState.Normal));
                    if (curOrientation == Orientation.Vertical)
                    {
                        curTop += curHeight;
                        if (i < numWindows - 2)
                        {
                            curWidth /= 2;
                        }
                        curOrientation = Orientation.Horizontal;
                    }
                    else
                    {
                        curLeft += curWidth;
                        if (i < numWindows - 2)
                        {
                            curHeight /= 2;
                        }
                        curOrientation = Orientation.Vertical;
                    }
                }
            }
            return list;
        }
        public void ShrinkPrimaryArea() { _primaryPercentOffset -= _primaryPercentIncrement; }
        public void ExpandPrimaryArea() { _primaryPercentOffset += _primaryPercentIncrement; }

        public void ResetPrimaryArea()
        {
            _primaryPercentOffset = 0;
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

        private int GetNumInPrimary()
        {
            return _numInPrimary + _numInPrimaryOffset;
        }
    }
}
