using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class TallLayoutEngine : ILayoutEngine
    {
        private readonly int _numInMaster;
        private readonly double _masterPercent;
        private readonly double _masterPercentIncrement;

        private int _numInMasterOffset = 0;
        private double _masterPercentOffset = 0;

        public TallLayoutEngine(int numInMaster, double masterPercent, double masterPercentIncrement)
        {
            _numInMaster = numInMaster;
            _masterPercent = masterPercent;
            _masterPercentIncrement = masterPercentIncrement;
        }

        public string Name => "tall";

        public IEnumerable<IWindowLocation> CalcLayout(int numWindows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();

            if (numWindows == 0)
                return list;

            int numInMaster = Math.Min(GetNumInMaster(), numWindows);

            int masterWidth = (int)(spaceWidth * (_masterPercent + _masterPercentOffset));
            int masterHeight = spaceHeight / numInMaster;
            int height = spaceHeight / Math.Max(numWindows - numInMaster, 1);

            // if there are more "master" windows than actual windows,
            // then we want the pane to actually spread the entire width
            // of the working area
            if (numInMaster >= numWindows)
            {
                masterWidth = spaceWidth;
            }

            int slaveWidth = spaceWidth - masterWidth;

            for (var i = 0; i < numWindows; i++)
            {
                if (i < numInMaster)
                {
                    list.Add(new WindowLocation(0, i * masterHeight, masterWidth, masterHeight, WindowState.Normal));
                }
                else
                {
                    list.Add(new WindowLocation(masterWidth, (i - numInMaster) * height, slaveWidth, height, WindowState.Normal));
                }
            }
            return list;
        }

        public void ShrinkMasterArea()
        {
            _masterPercentOffset -= _masterPercentIncrement;
        }

        public void ExpandMasterArea()
        {
            _masterPercentOffset += _masterPercentIncrement;
        }

        public void ResetMasterArea()
        {
            _masterPercentOffset = 0;
        }

        public void IncrementNumInMaster()
        {
            _numInMasterOffset++;
        }

        public void DecrementNumInMaster()
        {
            if (GetNumInMaster() > 1)
            {
                _numInMasterOffset--;
            }
        }

        private int GetNumInMaster()
        {
            return _numInMaster + _numInMasterOffset;
        }
    }
}
