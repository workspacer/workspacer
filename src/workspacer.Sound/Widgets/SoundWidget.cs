using System;
using System.Diagnostics;
using System.Linq;
using workspacer.Bar;

namespace workspacer.Sound.Widgets
{
    public interface ISoundWidget
    {
        void SetVolume(float scalarVolume);
        void SetMuted(bool isMuted);
        void DeviceChanged();

        DeviceInfo DeviceInfo { get; set; }
    }

    public class SoundWidget : BarWidgetBase, ISoundWidget
    {
        public bool RenderMultiColor { get; set; }
        public Color PrimaryColor { get; set; } = Color.White;
        public DeviceInfo DeviceInfo { get; set; }

        public SoundWidget(DeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;

            if (DeviceInfo == null || (string.IsNullOrEmpty(DeviceInfo.Id) && (DeviceInfo.Roles == null || !DeviceInfo.Roles.Any()) && !DeviceInfo.Type.HasValue))
            {
                throw new Exception("No device data given");
            }
        }

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

            var icons = new[]
            {
                "1F568", // Speaker no sound
                "1F569", // Speaker one sound
                "1F56A" // Speaker three sound
            };

            var iconCode = int.Parse(icons.GetItemFromValueSet(volume, 100, 0), System.Globalization.NumberStyles.HexNumber);
            return Part(char.ConvertFromUtf32(iconCode), RenderMultiColor ? GetVolumeColor(volume) : PrimaryColor);
        }

        private IBarWidgetPart RenderPercentage()
        {
            var volume = _volume;
            return Part($"{volume}%", RenderMultiColor ? GetVolumeColor(volume) : PrimaryColor);
        }

        private static Color GetVolumeColor(int volume)
        {
            var colors = new[]
            {
                Color.White,
                Color.Green,
                Color.Yellow,
                new Color(255, 127, 0),
                Color.Red,
            };

            return colors.GetItemFromValueSet(volume, 100, 0);
        }

        public override void Initialize()
        {
            SoundPlugin.Subscribe(this);
        }

        public void SetVolume(float scalarVolume)
        {
            _volume = (int)Math.Ceiling(100 * scalarVolume);
            MarkDirty();
        }

        public void SetMuted(bool isMuted)
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
