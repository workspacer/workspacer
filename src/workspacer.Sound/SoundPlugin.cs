using System.Collections.Generic;

namespace workspacer.Sound
{
    public class SoundPlugin : IPlugin
    {
        private static SoundClient _clientInstance = new SoundClient();

        private readonly SoundPluginConfig _config;
        private static List<ISoundEventCallback> _callbacks = new List<ISoundEventCallback>();

        public SoundPlugin() : this(new SoundPluginConfig()) { }
        public SoundPlugin(SoundPluginConfig config)
        {
            _clientInstance.VolumeChanged += VolumeChangedEvent;
            _clientInstance.MuteChanged += MuteChangedEvent;

            _config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeUp, VolumeStepUp);
            context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeDown, VolumeStepDown);
            context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeMute, ToggleMute);
        }

        private void VolumeChangedEvent(float val)
        {
            foreach(var callback in _callbacks)
            {
                callback.VolumeChanged(val);
            }
        }

        private void MuteChangedEvent(bool isMuted)
        {
            foreach (var callback in _callbacks)
            {
                callback.MuteChanged(isMuted);
            }
        }

        public static void Subscribe(ISoundEventCallback callback)
        {
            _callbacks.Add(callback);
            _clientInstance.ForceRefresh();
        }

        public void SetVolumeScalar(float value)
        {
            _clientInstance.SetVolumeScalar(value);
        }

        public void VolumeStepUp()
        {
            _clientInstance.VolumeStepUp();
        }

        public void VolumeStepDown()
        {
            _clientInstance.VolumeStepDown();
        }

        public void ToggleMute()
        {
            var isMuted = _clientInstance.GetMuted();
            if (!isMuted.HasValue)
            {
                return;
            }

            _clientInstance.SetMutedState(!isMuted.Value);
        }
    }
}
