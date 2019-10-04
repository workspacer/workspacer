using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public class Color : IEquatable<Color>
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public Color(byte r, byte g, byte b)
        {
            R = r; G = g; B = b;
        }

        public Color(int r, int g, int b)
        {
            ValidateRange(r, nameof(r));
            ValidateRange(g, nameof(g));
            ValidateRange(b, nameof(b));

            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }

        private void ValidateRange(int value, string name)
        {
            if (value < 0 || value > 255)
            {
                throw new ArgumentOutOfRangeException(name, "Color-component must be within 8-bit range!");
            }
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

        public bool Equals(Color other)
        {
            if (other == null)
                return false;

            return R == other.R &&
                   G == other.G &&
                   B == other.B;
        }

        public override bool Equals(object o)
        {
            return this.Equals(o as Color);
        }

        public override int GetHashCode()
        {
            int hash = (((R * 256) + B) * 256) + G;
            return hash;
        }
    }
}
