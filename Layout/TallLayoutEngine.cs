using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Layout
{
    public class TallLayoutEngine : ILayoutEngine
    {
        private int _numInMaster;
        private double _masterPercent;

        public TallLayoutEngine(int numInMaster, double masterPercent)
        {
            _numInMaster = numInMaster;
            _masterPercent = masterPercent;
        }

        public IEnumerable<IWindowLocation> CalcLayout(int numWindows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();

            if (numWindows == 0)
                return list;

            int width = (int)(spaceWidth * _masterPercent);
            int masterHeight = spaceHeight / _numInMaster;
            int height = spaceHeight / (numWindows - _numInMaster);

            // if there are more "master" windows than actual windows,
            // then we want the pane to actually spread the entire width
            // of the working area
            if (_numInMaster > numWindows)
            {
                width = spaceWidth;
            }

            for (var i = 0; i < numWindows; i++)
            {
                if (i < _numInMaster)
                {
                    list.Add(new WindowLocation(0, i * masterHeight, width, masterHeight));
                }
                else
                {
                    list.Add(new WindowLocation(width, (i - _numInMaster) * height, width, height));
                }
            }
            return list;
        }
    }
}
