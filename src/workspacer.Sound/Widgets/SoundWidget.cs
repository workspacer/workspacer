using System;
using workspacer.Bar;

namespace workspacer.Sound.Widgets
{
    public class SoundWidget : BarWidgetBase, ISoundEventCallback
    {
        public bool RenderMultiColor { get; set; }
        public Color PrimaryColor { get; set; } = Color.White;

        private int _volume;
        private bool _isMuted;

        public override IBarWidgetPart[] GetParts()
        {
            return new IBarWidgetPart[]
            {
                RenderIcon(),
                RenderPercentage()
            };
        }

        private IBarWidgetPart RenderIcon()
        {
            var volume = _volume;
            if (_isMuted)
            {
                return Part("/", PrimaryColor);
            }

            var icon = $"1F568";
            if (volume > 20)
            {
                //Speaker no sound
                icon = "1F568";
            }
            if (volume > 50)
            {
                //Speaker one sound
                icon = "1F569";
            }
            if (volume > 70)
            {
                //Speaker three sound
                icon = "1F56A";
            }

            var iconCode = int.Parse(icon, System.Globalization.NumberStyles.HexNumber);
            return Part(char.ConvertFromUtf32(iconCode), RenderMultiColor ? GetVolumeColor(volume) : PrimaryColor);
        }

        private IBarWidgetPart RenderPercentage()
        {
            var volume = _volume;
            if (_isMuted)
            {
                return Part("/", PrimaryColor);
            }

            return Part($"{volume}%", RenderMultiColor ? GetVolumeColor(volume) : PrimaryColor);
        }

        private Color GetVolumeColor(int volume)
        {
            var color = Color.White;

            if (volume > 0)
            {
                color = Color.Green;

                if (volume > 50)
                {
                    color = Color.Yellow;
                }
                if (volume > 70)
                {
                    color = new Color(255, 127, 0); // orange
                }
                if (volume > 90)
                {
                    color = Color.Red;
                }
            }

            return color;
        }

        public override void Initialize()
        {
            SoundPlugin.Subscribe(this);
        }

        public void VolumeChanged(float scalarVolume)
        {
            _volume = (int)Math.Ceiling(100 * scalarVolume);
        }
        public void MuteChanged(bool isMuted)
        {
            _isMuted = isMuted;
        }

        public void DeviceChanged()
        {
        }
    }
}
