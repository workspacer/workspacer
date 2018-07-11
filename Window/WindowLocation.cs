using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class WindowLocation : IWindowLocation
    {
        public WindowLocation(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; private set;}
        public int Y { get; private set;}
        public int Width { get; private set;}
        public int Height { get; private set;}

        public override string ToString()
        {
            return $"{X}:{Y}/{Width}:{Height}";
        }
    }
}
