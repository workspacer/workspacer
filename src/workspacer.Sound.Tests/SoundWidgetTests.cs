using System;
using System.Linq;
using workspacer.Sound.Widgets;
using Xunit;

namespace workspacer.Sound.Tests
{
    public class SoundWidgetTests
    {
        /// <summary>
        /// Assess that full volume output is correct
        /// </summary>
        [Fact]
        public void SoundWidget_HasCorrectRendering_100_NotMuted()
        {
            var widget = new SoundWidget(new DeviceInfo { Id = Guid.NewGuid().ToString() })
            {
                RenderMultiColor = false
            };

            widget.SetMuted(false);
            widget.SetVolume(1);

            var result = widget.GetParts();

            var iconCode = int.Parse("1F56A", System.Globalization.NumberStyles.HexNumber);

            Assert.Equal(2, result.Length);
            Assert.Equal("100%", result[0].Text);
            Assert.Equal(char.ConvertFromUtf32(iconCode), result[1].Text);
            Assert.True(result.All(x => x.ForegroundColor == widget.PrimaryColor));
        }

        /// <summary>
        /// Assess that muted output is correct
        /// </summary>
        [Fact]
        public void SoundWidget_HasCorrectRendering_Muted()
        {
            var widget = new SoundWidget(new DeviceInfo { Id = Guid.NewGuid().ToString() })
            {
                RenderMultiColor = false
            };

            widget.SetMuted(true);
            widget.SetVolume(1);

            var result = widget.GetParts();

            Assert.Single(result);
            Assert.Equal("/", result[0].Text);
            Assert.True(result.All(x => x.ForegroundColor == widget.PrimaryColor));
        }

        /// <summary>
        /// Assess that multicolor output is correct
        /// </summary>
        [Fact]
        public void SoundWidget_HasCorrectRendering_Colors()
        {
            var widget = new SoundWidget(new DeviceInfo { Id = Guid.NewGuid().ToString() })
            {
                RenderMultiColor = true
            };

            widget.SetMuted(false);

            widget.SetVolume(0);
            Assert.True(widget.GetParts().All(x => Equals(x.ForegroundColor, Color.White)));

            widget.SetVolume(0.25f);
            Assert.True(widget.GetParts().All(x => Equals(x.ForegroundColor, Color.Green)));

            widget.SetVolume(0.5f);
            Assert.True(widget.GetParts().All(x => Equals(x.ForegroundColor, Color.Yellow)));

            widget.SetVolume(0.75f);
            Assert.True(widget.GetParts().All(x => Equals(x.ForegroundColor, new Color(255, 127, 0))));

            widget.SetVolume(1f);
            Assert.True(widget.GetParts().All(x => Equals(x.ForegroundColor, Color.Red)));
        }
    }
}
