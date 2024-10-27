using System;
using Vannatech.CoreAudio.Constants;
using Vannatech.CoreAudio.Externals;
using Vannatech.CoreAudio.Interfaces;

namespace workspacer.Sound.Endpoints
{
    public class SoundClient : IAudioEndpointVolumeCallback
    {
        public event DeviceDataEventDelegate<float> VolumeChanged;
        public event DeviceDataEventDelegate<bool> MuteChanged;

        private IMMDevice _device;

        public DeviceInfo DeviceInfo { get; set; }

        private IAudioEndpointVolume _deviceAudioEndpointVolume;

        public void SetDevice(IMMDevice device)
        {
            if(device == null)
            {
                throw new Exception("No device given");
            }

            _device = device;

            // Fetch IAudioEndpointVolume
            _device.Activate(new Guid(ComIIDs.IAudioEndpointVolumeIID), (uint)CLSCTX.CLSCTX_INPROC_SERVER, IntPtr.Zero, out var volObject);
            if (volObject is IAudioEndpointVolume)
            {
                _deviceAudioEndpointVolume = volObject as IAudioEndpointVolume;
                _deviceAudioEndpointVolume.RegisterControlChangeNotify(this);
                return;
            }

            throw new Exception("InitializeAudioEnpoint Failed");
        }

        //Notify on default device change
        public int OnNotify(IntPtr notificationData)
        {
            UpdateVolume();
            UpdateMuted();

            return 0;
        }

        public void Refresh()
        {
            UpdateVolume();
            UpdateMuted();
        }

        private void UpdateVolume()
        {
            var scalar = GetVolume();
            VolumeChanged?.Invoke(DeviceInfo, scalar);
        }

        private void UpdateMuted()
        {
            var isMuted = GetMuted();
            MuteChanged?.Invoke(DeviceInfo, isMuted);
        }

        public bool GetMuted()
        {
            _deviceAudioEndpointVolume.GetMute(out var isMuted);
            return isMuted;
        }

        public float GetVolume()
        {
            _deviceAudioEndpointVolume.GetMasterVolumeLevelScalar(out var scalar);
            return scalar;
        }

        public int SetVolumeScalar(float value)
        {
            return _deviceAudioEndpointVolume.SetMasterVolumeLevelScalar(value, Guid.NewGuid());
        }

        public int VolumeStepUp()
        {
            return _deviceAudioEndpointVolume.VolumeStepUp(Guid.NewGuid());
        }

        public int VolumeStepDown()
        {
            return _deviceAudioEndpointVolume.VolumeStepDown(Guid.NewGuid());
        }

        public int SetMutedState(bool muted)
        {
            return _deviceAudioEndpointVolume.SetMute(muted, Guid.NewGuid());
        }
    }
}
