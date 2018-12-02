using System;

namespace workspacer
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,

        LControl = 1,
        RControl = 2,
        Control = LControl | RControl,

        LShift = 4,
        RShift = 8,
        Shift = LShift | RShift,

        LAlt = 16,
        RAlt = 32,
        Alt = LAlt | RAlt,

        LWin = 64,
        RWin = 128,
        Win = LWin | RWin
    }
}