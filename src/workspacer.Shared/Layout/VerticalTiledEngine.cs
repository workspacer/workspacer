using System;
using System.Collections.Generic;
using System.Linq;

namespace workspacer.Shared.Layout
{
    public class VerticalTiledEngine : ILayoutEngine
	{
        private readonly int _numInPrimary;
        private readonly double _primaryPercent;
        private readonly double _primaryPercentIncrement;
        private readonly int _maxRows;

        private int _numInPrimaryOffset;
        private double _primaryPercentOffset;

		public string Name => "vertical";

		public VerticalTiledEngine(int numInPrimary, double primaryPercent, double primaryPercentIncrement, int maxRows)
		{
			_numInPrimary = numInPrimary;
			_primaryPercent = primaryPercent;
			_primaryPercentIncrement = primaryPercentIncrement;
            _maxRows = maxRows;
        }

		public VerticalTiledEngine() : this(1, 0.5, 0.03, 3) { }

		public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
		{
            var locationList = new List<IWindowLocation>();
            var windowList = windows.ToList();
            var numWindows = windowList.Count;

            if (numWindows == 0)
                return locationList;

            var numInPrimary = Math.Min(GetNumInPrimary(), numWindows);
            var numInSecondary = numWindows - numInPrimary;
			var primaryWidth = spaceWidth;

            int CalcHeight(int mon, int n) => (int)(mon * (_maxRows == n + 1 ? 1 : _primaryPercent + _primaryPercentOffset));

            //var primaryHeight = CalcHeight(spaceHeight, 0);
            //var remainingHeight = spaceHeight - primaryHeight;


            var primaryHeight = 0;
            var remainingHeight = spaceHeight;
            var primaryHeightOffset = 0;
            for (var i = 0; i < numInPrimary; ++i)
            {
               primaryHeight = CalcHeight(remainingHeight, i);
               remainingHeight -= primaryHeight;
               locationList.Add(new WindowLocation(0, primaryHeightOffset, primaryWidth, primaryHeight, WindowState.Normal));
               primaryHeightOffset += primaryHeight;
            }

            var rowsAvailable = _maxRows - numInPrimary;
            if (rowsAvailable < 1)
            {
                for (var i = 0; i < numInSecondary; ++i)
                {
                    locationList.Add(new WindowLocation(0, 0, primaryWidth, primaryHeight, WindowState.Minimized));
                }
                return locationList;
            }

            var columnsUsed = 0;
            var secondaryIndex = new List<LayoutLocation>();

            for (var i = 0; i < numInSecondary; ++i)
            {
                var horizontalIndex = columnsUsed;
                var verticalIndex = i % rowsAvailable;
                var widthDivisor = horizontalIndex + 1;
                var width = spaceWidth / widthDivisor;

                if (widthDivisor > 1)
                {
                    foreach (var idx in secondaryIndex.FindAll(si => si.Y == verticalIndex))
                    {
                        idx.W = width;
                    }
                }

                if ((i + 1) % rowsAvailable == 0)
                {
                    ++columnsUsed;
                }

                secondaryIndex.Add(new LayoutLocation {X = horizontalIndex, Y = verticalIndex, W = width});
            }

            if (numInSecondary == 0)
            {
                return locationList;
            }

            var secondaryHeight = numInSecondary > rowsAvailable
                    ? remainingHeight / rowsAvailable
                    : remainingHeight / numInSecondary;

            foreach (var idx in secondaryIndex)
            {
                var horizontalPosition = idx.X * idx.W;
                var verticalPosition = primaryHeightOffset + (idx.Y * secondaryHeight);
                locationList.Add(new WindowLocation(horizontalPosition, verticalPosition, idx.W, secondaryHeight, WindowState.Normal));
            }

            return locationList;
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

        private class LayoutLocation
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int W { get; set; }
        }
	}
}
