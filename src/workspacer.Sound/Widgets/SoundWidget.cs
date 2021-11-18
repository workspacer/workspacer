using System;
using System.Diagnostics;
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
            if (_isMuted)
            {
                return Parts(Part("/", PrimaryColor, null, () => Process.Start("Rundll32.exe", @"shell32.dll,Control_RunDLL Mmsys.cpl,,0")));
            }

            return new IBarWidgetPart[]
            {
                RenderPercentage(),
                RenderIcon()
            };
        }

        private IBarWidgetPart RenderIcon()
        {
            var volume = _volume;

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
            MarkDirty();
        }
        public void MuteChanged(bool isMuted)
        {
            _isMuted = isMuted;
            MarkDirty();
        }

        public void DeviceChanged()
        {
            MarkDirty();
        }
    }
}
