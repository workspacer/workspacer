using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public delegate void WinEventDelegate(IntPtr hWinEventHook, Win32.EVENT_CONSTANTS eventType, IntPtr hwnd, Win32.OBJID idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
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

        public enum EVENT_CONSTANTS : uint
        {
            WINEVENT_OUTOFCONTEXT = 0x0000, // Events are ASYNC
            WINEVENT_SKIPOWNTHREAD = 0x0001, // Don't call back for events on installer's thread
            WINEVENT_SKIPOWNPROCESS = 0x0002, // Don't call back for events on installer's process
            WINEVENT_INCONTEXT = 0x0004, // Events are SYNC, this causes your dll to be injected into every process
            EVENT_MIN = 0x00000001,
            EVENT_MAX = 0x7FFFFFFF,
            EVENT_SYSTEM_SOUND = 0x0001,
            EVENT_SYSTEM_ALERT = 0x0002,
            EVENT_SYSTEM_FOREGROUND = 0x0003,
            EVENT_SYSTEM_MENUSTART = 0x0004,
            EVENT_SYSTEM_MENUEND = 0x0005,
            EVENT_SYSTEM_MENUPOPUPSTART = 0x0006,
            EVENT_SYSTEM_MENUPOPUPEND = 0x0007,
            EVENT_SYSTEM_CAPTURESTART = 0x0008,
            EVENT_SYSTEM_CAPTUREEND = 0x0009,
            EVENT_SYSTEM_MOVESIZESTART = 0x000A,
            EVENT_SYSTEM_MOVESIZEEND = 0x000B,
            EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C,
            EVENT_SYSTEM_CONTEXTHELPEND = 0x000D,
            EVENT_SYSTEM_DRAGDROPSTART = 0x000E,
            EVENT_SYSTEM_DRAGDROPEND = 0x000F,
            EVENT_SYSTEM_DIALOGSTART = 0x0010,
            EVENT_SYSTEM_DIALOGEND = 0x0011,
            EVENT_SYSTEM_SCROLLINGSTART = 0x0012,
            EVENT_SYSTEM_SCROLLINGEND = 0x0013,
            EVENT_SYSTEM_SWITCHSTART = 0x0014,
            EVENT_SYSTEM_SWITCHEND = 0x0015,
            EVENT_SYSTEM_MINIMIZESTART = 0x0016,
            EVENT_SYSTEM_MINIMIZEEND = 0x0017,
            EVENT_SYSTEM_DESKTOPSWITCH = 0x0020,
            EVENT_SYSTEM_END = 0x00FF,
            EVENT_OEM_DEFINED_START = 0x0101,
            EVENT_OEM_DEFINED_END = 0x01FF,
            EVENT_UIA_EVENTID_START = 0x4E00,
            EVENT_UIA_EVENTID_END = 0x4EFF,
            EVENT_UIA_PROPID_START = 0x7500,
            EVENT_UIA_PROPID_END = 0x75FF,
            EVENT_CONSOLE_CARET = 0x4001,
            EVENT_CONSOLE_UPDATE_REGION = 0x4002,
            EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003,
            EVENT_CONSOLE_UPDATE_SCROLL = 0x4004,
            EVENT_CONSOLE_LAYOUT = 0x4005,
            EVENT_CONSOLE_START_APPLICATION = 0x4006,
            EVENT_CONSOLE_END_APPLICATION = 0x4007,
            EVENT_CONSOLE_END = 0x40FF,
            EVENT_OBJECT_CREATE = 0x8000, // hwnd ID idChild is created item
            EVENT_OBJECT_DESTROY = 0x8001, // hwnd ID idChild is destroyed item
            EVENT_OBJECT_SHOW = 0x8002, // hwnd ID idChild is shown item
            EVENT_OBJECT_HIDE = 0x8003, // hwnd ID idChild is hidden item
            EVENT_OBJECT_REORDER = 0x8004, // hwnd ID idChild is parent of zordering children
            EVENT_OBJECT_FOCUS = 0x8005, // hwnd ID idChild is focused item
            EVENT_OBJECT_SELECTION = 0x8006, // hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
            EVENT_OBJECT_SELECTIONADD = 0x8007, // hwnd ID idChild is item added
            EVENT_OBJECT_SELECTIONREMOVE = 0x8008, // hwnd ID idChild is item removed
            EVENT_OBJECT_SELECTIONWITHIN = 0x8009, // hwnd ID idChild is parent of changed selected items
            EVENT_OBJECT_STATECHANGE = 0x800A, // hwnd ID idChild is item w/ state change
            EVENT_OBJECT_LOCATIONCHANGE = 0x800B, // hwnd ID idChild is moved/sized item
            EVENT_OBJECT_NAMECHANGE = 0x800C, // hwnd ID idChild is item w/ name change
            EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D, // hwnd ID idChild is item w/ desc change
            EVENT_OBJECT_VALUECHANGE = 0x800E, // hwnd ID idChild is item w/ value change
            EVENT_OBJECT_PARENTCHANGE = 0x800F, // hwnd ID idChild is item w/ new parent
            EVENT_OBJECT_HELPCHANGE = 0x8010, // hwnd ID idChild is item w/ help change
            EVENT_OBJECT_DEFACTIONCHANGE = 0x8011, // hwnd ID idChild is item w/ def action change
            EVENT_OBJECT_ACCELERATORCHANGE = 0x8012, // hwnd ID idChild is item w/ keybd accel change
            EVENT_OBJECT_INVOKED = 0x8013, // hwnd ID idChild is item invoked
            EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014, // hwnd ID idChild is item w? test selection change
            EVENT_OBJECT_CONTENTSCROLLED = 0x8015,
            EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016,
            EVENT_OBJECT_CLOAKED = 0x8017,
            EVENT_OBJECT_UNCLOAKED = 0x8018,
            EVENT_OBJECT_END = 0x80FF,
            EVENT_AIA_START = 0xA000,
            EVENT_AIA_END = 0xAFFF,
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(Win32.EVENT_CONSTANTS eventMin, Win32.EVENT_CONSTANTS eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);


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

        [Flags()]
        public enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
            /// contents of the client area are saved and copied back into the client area after the window is sized or 
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

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

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindowAsync(IntPtr hWnd, SW nCmdShow);

		public enum SW
		{
			SW_FORCEMINIMIZE = 11,
			SW_SHOW = 5,
			SW_SHOWNA = 8,
			SW_SHOWNOACTIVATE = 4,
			SW_SHOWMINNOACTIVE = 7,
			SW_SHOWMAXIMIZED = 3,
			SW_HIDE = 0,
			SW_RESTORE = 9,
			SW_MINIMIZE = 6,
			SW_SHOWMINIMIZED = 2,
			SW_SHOWNORMAL = 1
		}

        [Flags]
        public enum SWP : uint
		{
			SWP_SHOWWINDOW = 0x0040,
			SWP_HIDEWINDOW = 0x0080,
			SWP_NOZORDER = 0x0004,
			SWP_NOREDRAW = 0x0008,
			SWP_NOACTIVATE = 0x0010,
			SWP_NOMOVE = 0x0002,
			SWP_NOSIZE = 0x0001,
			SWP_FRAMECHANGED = 0x0020,
			SWP_NOCOPYBITS = 0x0100,
			SWP_NOOWNERZORDER = 0x0200,
			SWP_DEFERERASE = 0x2000,
			SWP_NOSENDCHANGING = 0x0400,
			SWP_ASYNCWINDOWPOS = 0x4000
		}

        [DllImport("user32.dll")]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

		[DllImport("user32.dll")]
		public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd,
			 [Optional] IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);
    }
}
