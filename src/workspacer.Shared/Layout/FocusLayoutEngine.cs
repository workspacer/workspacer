﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace workspacer
{
    // a layout that put the master view in the center of the screen, and the secondary windows on both sides, new windows opening on the right side
    public class FocusLayoutEngine : ILayoutEngine
    {
        public string Name { get; set; } = "focus";

        private readonly int _numInPrimary;
        private readonly double _primaryPercent;
        private readonly double _primaryPercentIncrement;

        private int _numInPrimaryOffset = 0;
        private double _primaryPercentOffset = 0;


        public FocusLayoutEngine() : this(1, 0.7, 0.03) { }

        public FocusLayoutEngine(int numInPrimary, double primaryPercent, double primaryPercentIncrement)
        {
            _numInPrimary = numInPrimary;
            _primaryPercent = primaryPercent;
            _primaryPercentIncrement = primaryPercentIncrement;

        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            int numInPrimary = Math.Min(GetNumInPrimary(), numWindows);
            int nbLeftWindows = GetNbLeftWindows(numWindows, numInPrimary);
            int nbRightWindows = GetNbRightWindows(numWindows, numInPrimary);

            int primaryWidth = (int)(spaceWidth * (_primaryPercent + _primaryPercentOffset));
            int secondaryWidth = (spaceWidth - primaryWidth) / 2;

            int primaryHeight = spaceHeight / numInPrimary;
            int leftHeight = GetSecondaryHeight(spaceHeight, nbLeftWindows);
            int rightHeight = GetSecondaryHeight(spaceHeight, nbRightWindows);


            // if there are more "primary" windows than actual windows,
            // then we want the pane to actually spread the entire width
            // of the working area
            if (numInPrimary >= numWindows)
            {
                primaryWidth = spaceWidth;
                secondaryWidth = 0;
            }
            else if (nbRightWindows == 0)
            {
                // fill up the right side if no windows
                primaryWidth += secondaryWidth;
            }


            for (var i = 0; i < numWindows; i++)
            {
                if (i < numInPrimary)
                {
                    list.Add(new WindowLocation(secondaryWidth, i * primaryHeight, primaryWidth, primaryHeight, WindowState.Normal));
                }
                else
                {
                    if (i < nbLeftWindows + numInPrimary)
                    {
                        // left side
                        list.Add(new WindowLocation(0, (i - numInPrimary) * leftHeight, secondaryWidth, leftHeight, WindowState.Normal));
                    }
                    else
                    {
                        // right side
                        list.Add(new WindowLocation(secondaryWidth + primaryWidth, (i - numInPrimary - nbLeftWindows) * rightHeight, secondaryWidth, rightHeight, WindowState.Normal));
                    }
                }
            }
            return list;
        }

        public int GetNbLeftWindows(int numWindows, int numInPrimary)
        {
            // +1 so that we round up
            return (numWindows - numInPrimary + 1) / 2;
        }

        public int GetNbRightWindows(int numWindows, int numInPrimary)
        {
            return (numWindows - numInPrimary) / 2; // rounded down
        }

        public int GetSecondaryHeight(int spaceHeight, int nbWindows)
        {
            return spaceHeight / Math.Max(nbWindows, 1);
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

    }
}
