using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tile.Net.Shared;

namespace Tile.Net
{
    public class Monitor : IMonitor
    {
        public int Index { get; private set; }
        public string Name => _screen.DeviceName;
        public int Width => _screen.WorkingArea.Width;
        public int Height => _screen.WorkingArea.Height;
        public int X => _screen.WorkingArea.X;
        public int Y => _screen.WorkingArea.Y;

        private Screen _screen;

        public Monitor(int index, Screen screen)
        {
            Index = index;
            _screen = screen;
        }
    }
}
