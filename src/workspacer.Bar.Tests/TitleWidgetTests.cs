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

        [Fact]
        public void GetTrimmedTitle_NoMax()
        {
            var s = "workspacer - Visual Studio Code";
            Assert.Equal("workspacer - Visual Studio Code", TitleWidget.GetTrimmedTitle(s));
        }

        [Fact]
        public void GetTrimmedTitle_EmptyString()
        {
            var s = "";
            Assert.Equal("", TitleWidget.GetTrimmedTitle(s, 10));
        }

        [Fact]
        public void GetTrimmedTitle_InputLessThenMax()
        {
            var s = "Visual Studio Code";
            Assert.Equal("Visual Studio Code", TitleWidget.GetTrimmedTitle(s, 50));
        }

        [Fact]
        public void GetTrimmedTitle_InputEqualToMax()
        {
            var s = "Visual Studio Code";
            Assert.Equal("Visual Studio Code", TitleWidget.GetTrimmedTitle(s, 18));
        }

        [Fact]
        public void GetTrimmedTitle_InputMoreThenMax()
        {
            var s = "Visual Studio Code";
            Assert.Equal("Vi...", TitleWidget.GetTrimmedTitle(s, 2));
        }

    }
}
