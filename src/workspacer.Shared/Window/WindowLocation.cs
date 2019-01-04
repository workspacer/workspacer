using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public class WindowLocation : IWindowLocation
    {
        public WindowLocation(int x, int y, int width, int height, WindowState state)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            State = state;
        }

        public int X { get; private set;}
        public int Y { get; private set;}
        public int Width { get; private set;}
        public int Height { get; private set;}
        public WindowState State { get; private set;}

        public bool IsPointInside(int x, int y)
        {
            return this.X <= x && x <= (this.X + this.Width) && this.Y <= y && y <= (this.Y + this.Height);
        }

        public override string ToString()
        {
            return $"{State} - {X}:{Y}/{Width}:{Height}";
        }
    }
}
