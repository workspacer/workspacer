using System;

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

        //NORD THEME COLORS
        //Polar Night
        public static readonly Color nord0 = new Color(46,52,64);
        public static readonly Color nord1 = new Color(59,66,82);
        public static readonly Color nord2 = new Color(67,76,94);
        public static readonly Color nord3 = new Color(76,86,106);
    
        //Snow Storm
        public static readonly Color nord4 = new Color(216,222,233);
        public static readonly Color nord5 = new Color(229,233,240);
        public static readonly Color nord6 = new Color(236,239,244);
    
        //Frost
        public static readonly Color nord7 = new Color(143, 188, 187);
        public static readonly Color nord8 = new Color(136, 192, 208);
        public static readonly Color nord9 = new Color(129, 161, 193);
        public static readonly Color nord10 = new Color(94, 129, 172);
    
        public static readonly Color nord11 = new Color(191, 97, 106);
        public static readonly Color nord12 = new Color(208, 135, 112);
        public static readonly Color nord13 = new Color(235, 203, 139);
        public static readonly Color nord14 = new Color(163, 190, 140);
        public static readonly Color nord15 = new Color(180, 142, 173);
        
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
