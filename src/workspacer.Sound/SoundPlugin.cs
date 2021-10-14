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

        public int? VolumeStepUp()
        {
            return _clientInstance?.VolumeStepUp();
        }

        public int? VolumeStepDown()
        {
            return _clientInstance?.VolumeStepDown();
        }

        public int SetMutedState(bool muted)
        {
            return _clientInstance.SetMutedState(muted);
        }
    }
}
