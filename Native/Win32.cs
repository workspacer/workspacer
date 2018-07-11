using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, Win32.OBJID idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
    public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

    public static class Win32
    {
        public class Message
        {
            public int message { get; set; }
        }
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        public static long CHILDID_SELF = 0;
        public enum OBJID { OBJID_WINDOW = 0 }

        public static int GWL_STYLE = -16;
        public static int GWL_EXSTYLE = -20;

        [Flags]
        public enum WS : uint
		{
			WS_OVERLAPPED = 0,
			WS_POPUP = 0x80000000,
			WS_CHILD = 0x40000000,
			WS_MINIMIZE = 0x20000000,
			WS_VISIBLE = 0x10000000,
			WS_DISABLED = 0x8000000,
			WS_CLIPSIBLINGS = 0x4000000,
			WS_CLIPCHILDREN = 0x2000000,
			WS_MAXIMIZE = 0x1000000,
			WS_CAPTION = WS_BORDER | WS_DLGFRAME,
			WS_BORDER = 0x800000,
			WS_DLGFRAME = 0x400000,
			WS_VSCROLL = 0x200000,
			WS_HSCROLL = 0x100000,
			WS_SYSMENU = 0x80000,
			WS_THICKFRAME = 0x40000,
			WS_MINIMIZEBOX = 0x20000,
			WS_MAXIMIZEBOX = 0x10000,
			WS_TILED = WS_OVERLAPPED,
			WS_ICONIC = WS_MINIMIZE,
			WS_SIZEBOX = WS_THICKFRAME,
			WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX
        }

        [Flags]
        public enum WS_EX : uint
		{
			WS_EX_DLGMODALFRAME = 0x0001,
			WS_EX_NOPARENTNOTIFY = 0x0004,
			WS_EX_TOPMOST = 0x0008,
			WS_EX_ACCEPTFILES = 0x0010,
			WS_EX_TRANSPARENT = 0x0020,
			WS_EX_MDICHILD = 0x0040,
			WS_EX_TOOLWINDOW = 0x0080,
			WS_EX_WINDOWEDGE = 0x0100,
			WS_EX_CLIENTEDGE = 0x0200,
			WS_EX_CONTEXTHELP = 0x0400,
			WS_EX_RIGHT = 0x1000,
			WS_EX_LEFT = 0x0000,
			WS_EX_RTLREADING = 0x2000,
			WS_EX_LTRREADING = 0x0000,
			WS_EX_LEFTSCROLLBAR = 0x4000,
			WS_EX_RIGHTSCROLLBAR = 0x0000,
			WS_EX_CONTROLPARENT = 0x10000,
			WS_EX_STATICEDGE = 0x20000,
			WS_EX_APPWINDOW = 0x40000,
			WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
			WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
			WS_EX_LAYERED = 0x00080000,
			WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
			WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
			WS_EX_COMPOSITED = 0x02000000,
			WS_EX_NOACTIVATE = 0x08000000
        }

        public enum GW : uint
        {
            GW_OWNER = 4,
        }


        public enum GA : uint
        {
            GA_PARENT = 1,
			GA_ROOT = 2,
			GA_ROOTOWNER = 3
		}

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TITLEBARINFO
        {
            public const int CCHILDREN_TITLEBAR = 5;
            public uint cbSize;
            public RECT rcTitleBar;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CCHILDREN_TITLEBAR + 1)]
            public uint[] rgstate;
        }

        public static uint EVENT_SYSTEM_MOVESIZESTART = 0xA;
        public static uint EVENT_SYSTEM_MOVESIZEEND = 0xB;
        public static uint EVENT_OBJECT_DESTROY = 0x00008001;
        public static uint EVENT_OBJECT_SHOW = 0x00008002;
        public static uint EVENT_OBJECT_HIDE = 0x00008003;
        public static uint EVENT_SYSTEM_FOREGROUND = 0x3;
        public static uint EVENT_SYSTEM_MINIMIZESTART = 0x16;
        public static uint EVENT_SYSTEM_MINIMIZEEND = 0x17;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);


        [DllImport("user32.dll")]
        public static extern bool GetMessage(ref Message lpMsg, IntPtr handle, uint mMsgFilterInMain, uint mMsgFilterMax);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);


        [DllImport("user32.dll", EntryPoint="SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern uint GetWindowLongPtr(IntPtr hWnd, int nIndex);

        public static WS GetWindowStyleLongPtr(IntPtr hwnd) { return (WS)GetWindowLongPtr(hwnd, GWL_STYLE); }
        public static WS_EX GetWindowExStyleLongPtr(IntPtr hwnd) { return (WS_EX)GetWindowLongPtr(hwnd, GWL_EXSTYLE); }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GW uCmd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, GA gaFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTitleBarInfo(IntPtr hwnd, ref TITLEBARINFO pti);
    }
}
