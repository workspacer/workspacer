using Xunit;

namespace workspacer.TitleBar.Tests
{
    public class TitleBarStyleTests
    {
        /// <summary>
        ///   Returns the most common window style. For more, see
        ///   https://docs.microsoft.com/en-us/windows/win32/winmsg/using-windows
        /// </summary>
        private static Win32.WS GetDefaultStyle()
        {
            return Win32.WS.WS_OVERLAPPEDWINDOW;
        }

        /// <summary>
        ///   Returns the window style with title bar and sizing border hidden.
        /// </summary>
        private static Win32.WS GetAllStylesOff()
        {
            // See https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles
            return Win32.WS.WS_OVERLAPPED | Win32.WS.WS_SYSMENU | Win32.WS.WS_MINIMIZEBOX | Win32.WS.WS_MAXIMIZEBOX;
        }

        /// <summary>
        ///   Make sure by default, we don't mess with Windows' default style.
        /// </summary>
        [Fact]
        public void TitleBarStyle_RespectWindowsDefaults()
        {
            var settings = new TitleBarStyle();
            Assert.True(settings.ShowTitleBar);
            Assert.True(settings.ShowSizingBorder);
        }

        /// <summary>
        ///   Make sure that if we set the styles to the Windows defaults, we get the same results.
        /// </summary>
        [Fact]
        public void TitleBarStyle_WindowsDefaults()
        {
            var settings = new TitleBarStyle();
            var style = GetDefaultStyle();
            var win32Style = TitleBarPlugin.UpdateStyle(style, settings);
            Assert.Equal(GetDefaultStyle(), win32Style);
        }

        /// <summary>
        ///  Hide the title bar.
        /// </summary>
        [Fact]
        public void TitleBarStyle_HideTitleBar()
        {
            var settings = new TitleBarStyle() { ShowTitleBar = false };
            var style = GetDefaultStyle();
            var win32Style = TitleBarPlugin.UpdateStyle(style, settings);
            Assert.Equal(GetDefaultStyle() & ~Win32.WS.WS_CAPTION, win32Style);
        }

        /// <summary>
        ///   Hide the sizing border.
        /// </summary>
        [Fact]
        public void TitleBarStyle_HideSizingBorder()
        {
            var settings = new TitleBarStyle() { ShowSizingBorder = false };
            var style = GetDefaultStyle();
            var win32Style = TitleBarPlugin.UpdateStyle(style, settings);
            Assert.Equal(GetDefaultStyle() & ~Win32.WS.WS_THICKFRAME, win32Style);
        }

        /// <summary>
        ///   Hide the title bar and the sizing border.
        /// </summary>
        [Fact]
        public void TitleBarStyle_HideTitleBarAndSizingBorder()
        {
            var settings = new TitleBarStyle() { ShowTitleBar = false, ShowSizingBorder = false };
            var style = GetDefaultStyle();
            var win32Style = TitleBarPlugin.UpdateStyle(style, settings);
            Assert.Equal(GetAllStylesOff(), win32Style);
        }

        /// <summary>
        ///   Show the title bar when it's hidden.
        /// </summary>
        [Fact]
        public void TitleBarStyle_ShowTitleBarWhenHidden()
        {
            var settings = new TitleBarStyle() { ShowTitleBar = true };
            var style = GetAllStylesOff();
            var win32Style = TitleBarPlugin.UpdateStyle(style, settings);
            Assert.Equal(GetDefaultStyle() | Win32.WS.WS_CAPTION, win32Style);
        }

        /// <summary>
        ///   Show the sizing border when it's hidden.
        /// </summary>
        [Fact]
        public void TitleBarStyle_ShowSizingBorderWhenHidden()
        {
            var settings = new TitleBarStyle() { ShowSizingBorder = true };
            var style = GetAllStylesOff();
            var win32Style = TitleBarPlugin.UpdateStyle(style, settings);
            Assert.Equal(GetDefaultStyle() | Win32.WS.WS_THICKFRAME, win32Style);
        }

        /// <summary>
        ///   Show the title bar and the sizing border when they're hidden.
        /// </summary>
        [Fact]
        public void TitleBarStyle_ShowTitleBarAndSizingBorderWhenHidden()
        {
            var settings = new TitleBarStyle() { ShowTitleBar = true, ShowSizingBorder = true };
            var style = GetAllStylesOff();
            var win32Style = TitleBarPlugin.UpdateStyle(style, settings);
            Assert.Equal(GetDefaultStyle(), win32Style);
        }
    }
}
