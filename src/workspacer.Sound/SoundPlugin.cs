using System.Collections.Generic;

namespace workspacer.Sound
{
    public class SoundPlugin : IPlugin
    {
        private IConfigContext _context;
        private SoundPluginConfig _config;
        private SoundClient _clientInstance;

        private static List<ISoundEventCallback> _callbacks = new List<ISoundEventCallback>();

        public SoundPlugin() : this(new SoundPluginConfig()) { }
        public SoundPlugin(SoundPluginConfig config)
        {
            _config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            _clientInstance = new SoundClient(_config, context);

            context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeUp, VolumeStepUp);
            context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeDown, VolumeStepDown);
            context.Keybinds.Subscribe(KeyModifiers.None, Keys.VolumeMute, ToggleMute);

            _clientInstance.VolumeChanged += VolumeChangedEvent;
            _clientInstance.MuteChanged += MuteChangedEvent;
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
        }

        public int SetVolumeScalar(float value)
        {
            return _clientInstance.SetVolumeScalar(value);
        }

        public void VolumeStepUp()
        {
            _clientInstance?.VolumeStepUp();
        }

        public void VolumeStepDown()
        {
            _clientInstance?.VolumeStepDown();
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
