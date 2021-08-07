﻿using System;
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
        private readonly bool _leftToRight;

        private int _numInPrimaryOffset = 0;
        private double _primaryPercentOffset = 0;

        public TallLayoutEngine(bool reversed = false) : this(1, 0.5, 0.03, reversed) { }

        public TallLayoutEngine(int numInPrimary, double primaryPercent, double primaryPercentIncrement, bool reversed)
        {
            _numInPrimary = numInPrimary;
            _primaryPercent = primaryPercent;
            _primaryPercentIncrement = primaryPercentIncrement;
            _leftToRight = !reversed;
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
            int height = spaceHeight / Math.Max(numWindows - numInPrimary, 1);

            // if there are more "primary" windows than actual windows,
            // then we want the pane to actually spread the entire width
            // of the working area
            if (numInPrimary >= numWindows)
            {
                primaryWidth = spaceWidth;
            }

            int secondaryWidth = spaceWidth - primaryWidth;

            for (var i = 0; i < numWindows; i++)
            {
                if (i < numInPrimary)
                {
                    list.Add(new WindowLocation(CalcXPos(0, primaryWidth, spaceWidth), i * primaryHeight, primaryWidth, primaryHeight, WindowState.Normal));
                }
                else
                {
                    list.Add(new WindowLocation(CalcXPos(primaryWidth, secondaryWidth, spaceWidth), (i - numInPrimary) * height, secondaryWidth, height, WindowState.Normal));
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

        private int CalcXPos(int x, int windowsWidth, int spaceWidth)
        {
            return _leftToRight ? x : spaceWidth - x - windowsWidth;
        }
    }
}
