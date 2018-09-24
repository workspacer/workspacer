using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.Bar
{
    public class Color : IColor
    {
        public int R { get; }
        public int G { get; }
        public int B { get; }

        public Color(int r, int g, int b)
        {
            R = r; G = g; B = b;
        }

        public static readonly Color White =     new Color(0xFF, 0xFF, 0xFF);
        public static readonly Color Silver =    new Color(0xC0, 0xC0, 0xC0);
        public static readonly Color Gray =      new Color(0x80, 0x80, 0x80);
        public static readonly Color Black =     new Color(0x00, 0x00, 0x00);
        public static readonly Color Red =       new Color(0xFF, 0x00, 0x00);
        public static readonly Color Maroon =    new Color(0x80, 0x00, 0x00);
        public static readonly Color Yellow =    new Color(0xFF, 0xFF, 0x00);
        public static readonly Color Olive =     new Color(0x80, 0x80, 0x00);
        public static readonly Color Lime =      new Color(0x00, 0xFF, 0x00);
        public static readonly Color Green =     new Color(0x00, 0x80, 0x00);
        public static readonly Color Aqua =      new Color(0x00, 0xFF, 0xFF);
        public static readonly Color Teal =      new Color(0x00, 0x80, 0x80);
        public static readonly Color Blue =      new Color(0x00, 0x00, 0xFF);
        public static readonly Color Navy =      new Color(0x00, 0x00, 0x80);
        public static readonly Color Fuchsia =   new Color(0xFF, 0x00, 0xFF);
        public static readonly Color Purple =    new Color(0x80, 0x00, 0x80);
    }
}
