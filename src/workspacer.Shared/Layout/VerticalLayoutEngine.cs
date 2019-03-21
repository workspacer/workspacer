using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Shared.Layout
{
	public class VerticalLayoutEngine : ILayoutEngine
	{
        private readonly int _numInPrimary;
        private readonly double _primaryPercent;
        private readonly double _primaryPercentIncrement;

        private int _numInPrimaryOffset;
        private double _primaryPercentOffset;

		public string Name => "vertical";

		public VerticalLayoutEngine(int numInPrimary, double primaryPercent, double primaryPercentIncrement)
		{
			_numInPrimary = numInPrimary;
			_primaryPercent = primaryPercent;
			_primaryPercentIncrement = primaryPercentIncrement;
		}

		public VerticalLayoutEngine() : this(1, 0.5, 0.03) { }

		public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
		{
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            var numInPrimary = Math.Min(GetNumInPrimary(), numWindows);

            //var primaryHeight = (int)(spaceHeight * (_primaryPercent + _primaryPercentOffset));
            //var primaryWidth = spaceWidth / numInPrimary;
            //var width = spaceWidth / Math.Max(numWindows - numInPrimary, 1);
			var primaryWidth = spaceWidth;
            var primaryHeight = (int)((spaceHeight * (_primaryPercent + _primaryPercentOffset)) / Math.Max(numInPrimary, 1));
            var remainingHeight = spaceHeight - primaryHeight;

			// TODO this, the for loop, secondary height
            // if there are more "primary" windows than actual windows,
            // then we want the pane to actually spread the entire height
            // of the working area
            if (numInPrimary >= numWindows)
            {
                primaryHeight = spaceHeight;
            }

            var secondaryHeight = remainingHeight / Math.Max(numWindows - numInPrimary, 1);

            for (var i = 0; i < numWindows; ++i)
            {
                if (i < numInPrimary)
                {
                    list.Add(new WindowLocation(0, i * primaryHeight, primaryWidth, primaryHeight, WindowState.Normal));
                }
                else
                {
                    list.Add(new WindowLocation(0, (i * primaryHeight) + ((numInPrimary - i) * secondaryHeight), primaryWidth, secondaryHeight, WindowState.Normal));
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
	}
}
