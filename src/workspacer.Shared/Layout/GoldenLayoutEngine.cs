using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public class GoldenLayoutEngine : ILayoutEngine
    {
        private readonly int _numInPrimary;
        private readonly double _primaryPercent;
        
        private int _numInPrimaryOffset = 0;
        private double _primaryPercentOffset = 0;
        
        public GoldenLayoutEngine() : this(1, 0.618) { }

        public GoldenLayoutEngine(int numInPrimary, double primaryPercent)
        {
            _numInPrimary = numInPrimary;
            _primaryPercent = primaryPercent;            
        }
        
        public string Name => "Golden";

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            var numInPrimary = Math.Min(GetNumInPrimary(), numWindows);

            var primaryWidth = (int)(spaceWidth * (_primaryPercent + _primaryPercentOffset));
            var primaryHeight = spaceHeight / numInPrimary;
            var height = spaceHeight / Math.Max(numWindows - numInPrimary, 1);

            if (numInPrimary >= numWindows)
            {
                primaryWidth = spaceWidth;
            }
                        
            var secondaryWidth = spaceWidth - primaryWidth;
            var secondaryHeight = (int)(spaceHeight * 0.618);
            var thirdHeight = spaceHeight - secondaryHeight;

            for (var i = 0; i < numWindows; i++)
            {
                if (i == 0)
                {
                    list.Add(new WindowLocation(0, i * primaryHeight, primaryWidth, primaryHeight, WindowState.Normal));
                }
                else if (i == 1 && numWindows == 2) 
                {
                    list.Add(new WindowLocation(primaryWidth, (i - numInPrimary) * height, secondaryWidth, spaceHeight, WindowState.Normal));
                }
                else if (i == 1 && numWindows > 2)
                {
                    list.Add(new WindowLocation(primaryWidth, (i - numInPrimary) * height, secondaryWidth, secondaryHeight, WindowState.Normal));
                }
                else
                {
                    list.Add(new WindowLocation(primaryWidth, secondaryHeight, secondaryWidth, thirdHeight, WindowState.Normal));
                }
            }
            return list;
        }

        private int GetNumInPrimary()
        {
            return _numInPrimary + _numInPrimaryOffset;
        }

        public void DecrementNumInPrimary()
        {            
        }

        public void ExpandPrimaryArea()
        {         
        }

        public void IncrementNumInPrimary()
        {         
        }

        public void ResetPrimaryArea()
        {            
        }

        public void ShrinkPrimaryArea()
        {            
        }
    }
}
