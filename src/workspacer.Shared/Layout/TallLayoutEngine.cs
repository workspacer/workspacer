using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public class TallLayoutEngine : ILayoutEngine
    {
        private readonly int _numInPrimary;
        private readonly double _primaryPercent;
        private readonly double _primaryPercentIncrement;

        private int _numInPrimaryOffset = 0;
        private double _primaryPercentOffset = 0;

        private int _gapSize = 0;

        public TallLayoutEngine() : this(1, 0.5, 0.03) { }

        public TallLayoutEngine(int numInPrimary, double primaryPercent, double primaryPercentIncrement)
        {
            _numInPrimary = numInPrimary;
            _primaryPercent = primaryPercent;
            _primaryPercentIncrement = primaryPercentIncrement;
        }

        public TallLayoutEngine(int numInPrimary, double primaryPercent, double primaryPercentIncrement, int gapSize)
        {
            _numInPrimary = numInPrimary;
            _primaryPercent = primaryPercent;
            _primaryPercentIncrement = primaryPercentIncrement;
            _gapSize = gapSize;
        }

        public string Name => "tall";

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            int numInPrimary = Math.Min(GetNumInPrimary(), numWindows);

            int primaryWidth = (int)(spaceWidth * (_primaryPercent + _primaryPercentOffset));
            int primaryHeight = spaceHeight / numInPrimary;

            // if there are more "primary" windows than actual windows,
            // then we want the pane to actually spread the entire width
            // of the working area
            if (numInPrimary >= numWindows)
            {
                primaryWidth = spaceWidth;
            }

            int secondayHeight = spaceHeight / Math.Max(numWindows - numInPrimary, 1);
            int secondaryWidth = spaceWidth - primaryWidth;

            for (var i = 0; i < numWindows; i++)
            {
                // gaps all around if there is only 1 primary window
                // or we are iterating on the last window in the primary section
                if (i == numInPrimary-1)
                {
                    list.Add(new WindowLocation(0 + _gapSize,
                        i * primaryHeight + _gapSize,
                        primaryWidth - _gapSize * 2,
                        primaryHeight - _gapSize * 2,
                        WindowState.Normal));
                }
                // more than one in primary so the first primary windows
                // lose the gap on the bottom
                else if (i < numInPrimary && numInPrimary > 1)
                {
                    list.Add(new WindowLocation(0 + _gapSize,
                        i * primaryHeight + _gapSize,
                        primaryWidth - _gapSize * 2,
                        primaryHeight - _gapSize,
                        WindowState.Normal));
                }
                //and only bottom if there are no other windows
                else if (i == numWindows-1)
                {
                    list.Add(new WindowLocation(primaryWidth,
                        (i - numInPrimary) * secondayHeight + _gapSize,
                        secondaryWidth - _gapSize,
                        secondayHeight - _gapSize * 2,
                        WindowState.Normal));
                }
                // Every other secondary window should have gaps top, left
                else
                {
                    list.Add(new WindowLocation(primaryWidth,
                        (i - numInPrimary) * secondayHeight + _gapSize,
                        secondaryWidth - _gapSize,
                        secondayHeight - _gapSize,
                        WindowState.Normal));
                }
            }
            return list;
        }

        public void ShrinkPrimaryArea()
        {
            _primaryPercentOffset -= _primaryPercentIncrement;
        }

        public void ExpandPrimaryArea()
        {
            _primaryPercentOffset += _primaryPercentIncrement;
        }

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

        public void IncrementGapSize()
        {
            if (_gapSize < 60)
                _gapSize += 2;
            else
                _gapSize = 60;
        }

        public void DecrementGapSize()
        {
            if (_gapSize > 0)
                _gapSize -= 2;
            else
                _gapSize = 0;
        }
    }
}
