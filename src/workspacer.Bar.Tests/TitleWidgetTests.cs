using workspacer.Bar.Widgets;
using Xunit;

namespace workspacer.Bar.Tests
{
    public class TitleWidgetTests
    {
        [Fact]
        public void GetShortTitle_SplitHyphen()
        {
            var s = "workspacer - Visual Studio Code";
            Assert.Equal("Visual Studio Code", TitleWidget.GetShortTitle(s));
        }

        [Fact]
        public void GetShortTitle_SplitEmDash()
        {
            var s = "workspacer — Visual Studio Code";
            Assert.Equal("Visual Studio Code", TitleWidget.GetShortTitle(s));
        }

        [Fact]
        public void GetShortTitle_SplitBar()
        {
            var s = "workspace | Visual Studio Code";
            Assert.Equal("Visual Studio Code", TitleWidget.GetShortTitle(s));
        }

        [Fact]
        public void GetShortTitle_NoSeparators()
        {
            var s = "Visual Studio Code";
            Assert.Equal("Visual Studio Code", TitleWidget.GetShortTitle(s));
        }

        [Fact]
        public void GetShortTitle_EmptyString()
        {
            var s = "";
            Assert.Equal("", TitleWidget.GetShortTitle(s));
        }
    }
}
