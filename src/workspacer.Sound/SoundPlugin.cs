using System;
using System.Collections.Generic;
using System.Linq;
using workspacer.Sound.Widgets;

namespace workspacer.Sound
{
    public class SoundPlugin : IPlugin
    {
        private static SoundManager _clientInstance = new();
        private static readonly List<ISoundWidget> _widgets = new();

        private readonly SoundPluginConfig _config;

        public SoundPlugin() : this(new SoundPluginConfig()) { }
        public SoundPlugin(SoundPluginConfig config)
        {
            _clientInstance.VolumeChanged += VolumeChangedEvent;
            _clientInstance.MuteChanged += MuteChangedEvent;

            _config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            if (_config.BindDefaultPlaybackKeybinds)
            {
                context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeUp, () => VolumeStepUp(_clientInstance.GetDefaultPlaybackDeviceId()));
                context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeDown, () => VolumeStepDown(_clientInstance.GetDefaultPlaybackDeviceId()));
                context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeMute, () => ToggleMute(_clientInstance.GetDefaultPlaybackDeviceId()));
            }
        }

        private void VolumeChangedEvent(DeviceInfo device, float val)
        {
            ApplyToWidgetsWithCriteria(device, (widget) => widget.SetVolume(val));
        }

        private void MuteChangedEvent(DeviceInfo device, bool isMuted)
        {
            ApplyToWidgetsWithCriteria(device, (widget) => widget.SetMuted(isMuted));
        }

        private void ApplyToWidgetsWithCriteria(DeviceInfo device, Action<ISoundWidget> action)
        {
            IEnumerable<ISoundWidget> widgets = null;
            if (string.IsNullOrEmpty(device.Id))
            {
                widgets = _widgets.Where(x => x.DeviceInfo.Roles.Any(p => device.Roles.Contains(p)) && x.DeviceInfo.Type == device.Type).ToList();
            }
            else
            {
                widgets = _widgets.Where(x => x.DeviceInfo.Id == device.Id).ToList();
            }

            foreach (var widget in widgets)
            {
                action(widget);
            }
        }

        public static void Subscribe(ISoundWidget widget)
        {
            _widgets.Add(widget);

            SoundClient client = null;
            if (string.IsNullOrEmpty(widget.DeviceInfo.Id) && widget.DeviceInfo.Roles.Any() && widget.DeviceInfo.Type.HasValue)
            {
                client = _clientInstance.RegisterClientForDefault(widget.DeviceInfo.Type.Value, widget.DeviceInfo.Roles.ToArray());
            }

            if (!string.IsNullOrEmpty(widget.DeviceInfo.Id))
            {
                client = _clientInstance.RegisterClientForDeviceId(widget.DeviceInfo.Id);
            }

            client?.Refresh();
        }

        public static void SetVolumeScalar(string deviceId, float value)
        {
            var clients = _clientInstance.GetClientsForDeviceId(deviceId);
            foreach (var client in clients)
            {
                client?.SetVolumeScalar(value);
            }
        }

        public static void VolumeStepUp(string deviceId)
        {
            var clients = _clientInstance.GetClientsForDeviceId(deviceId);
            foreach (var client in clients)
            {
                client?.VolumeStepUp();
            }
        }

        public static void VolumeStepDown(string deviceId)
        {
            var clients = _clientInstance.GetClientsForDeviceId(deviceId);
            foreach (var client in clients)
            {
                client?.VolumeStepDown();
            }
        }

        public static void ToggleMute(string deviceId)
        {
            var clients = _clientInstance.GetClientsForDeviceId(deviceId);
            foreach (var client in clients)
            {
                var isMuted = client?.GetMuted();
                if (!isMuted.HasValue)
                {
                    return;
                }

                client.SetMutedState(!isMuted.Value);
            }
        }
    }
}
