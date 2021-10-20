using System;
using Vannatech.CoreAudio.Constants;
using Vannatech.CoreAudio.Enumerations;
using Vannatech.CoreAudio.Externals;
using Vannatech.CoreAudio.Interfaces;

namespace workspacer.Sound
{
    public delegate void VolumeChangedDelegate(float value);
    public delegate void MuteChangedDelegate(bool isMuted);
    public delegate void DeviceChangedDelegate(string deviceId, uint newState);

    public class SoundClient : IMMNotificationClient, IAudioEndpointVolumeCallback
    {
        private IMMDeviceEnumerator _mmDeviceEnumerator;
        private IMMDevice _defaultDevice;
        private IAudioEndpointVolume _defaultDeviceAudioEndpointVolume;

        public event VolumeChangedDelegate VolumeChanged;
        public event MuteChangedDelegate MuteChanged;
        public event DeviceChangedDelegate DeviceChanged;

        public SoundClient()
        {
            _mmDeviceEnumerator = Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComCLSIDs.MMDeviceEnumeratorCLSID))) as IMMDeviceEnumerator;
            if (_mmDeviceEnumerator == null)
            {
                throw new Exception("Creating device enumerator failed");
            }

            _mmDeviceEnumerator.RegisterEndpointNotificationCallback(this);

            GetDefaultDevice();
            ForceRefresh();
        }

        public void ForceRefresh()
        {
            CheckVolumeState();
            CheckMuteState();
        }

        #region System Events

        public void OnDeviceStateChanged(string deviceId, uint newState)
        {
            DeviceChanged.Invoke(deviceId, newState);
#if EVENT_DEBUG
            MessageBox.Show($"DeviceStateChanged deviceId:[{deviceId}] newState:[{newState}]");
#endif
        }

        public void OnDeviceAdded(string deviceId)
        {

#if EVENT_DEBUG
            MessageBox.Show($"DeviceAdded DeviceId:[{deviceId}]");
#endif
        }

        public void OnDeviceRemoved(string deviceId)
        {

#if EVENT_DEBUG
            MessageBox.Show($"DeviceRemoved DeviceId:[{deviceId}]");
#endif
        }

        public void OnDefaultDeviceChanged(EDataFlow dataFlow, ERole deviceRole, string defaultDeviceId)
        {
#if EVENT_DEBUG
            MessageBox.Show($"DeviceRemoved DataFlow:[{dataFlow}] DeviceRole:[{deviceRole}] DeviceId:[{defaultDeviceId}]");
#endif
            if (dataFlow == EDataFlow.eRender && deviceRole == ERole.eMultimedia)
            {
                GetDefaultDevice(defaultDeviceId);
                ForceRefresh();
            }
        }

        public void OnPropertyValueChanged(string deviceId, PROPERTYKEY propertyKey)
        {
            // To be implemented
#if EVENT_DEBUG
            MessageBox.Show($"DeviceStateChanged deviceId:[{deviceId}] propertyKey:[{propertyKey}]");
#endif
        }

        //Notify on default device change
        public int OnNotify(IntPtr notificationData)
        {

#if EVENT_DEBUG
            MessageBox.Show($"OnNotify {notificationData}");
#endif
            if (_defaultDeviceAudioEndpointVolume == null)
            {
                return -1;
            }

            CheckVolumeState();
            CheckMuteState();

            return 0;
        }
#endregion

        private void GetDefaultDevice(string defaultDeviceId = null)
        {
            if (_mmDeviceEnumerator == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(defaultDeviceId))
            {
                _mmDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out _defaultDevice);
            }
            else
            {
                _mmDeviceEnumerator.GetDevice(defaultDeviceId, out _defaultDevice);
            }

            if (_defaultDevice == null)
            {
                return;
            }

            // Fetch IAudioEndpointVolume
            object volObject;
            _defaultDevice.Activate(new Guid(ComIIDs.IAudioEndpointVolumeIID), (uint)CLSCTX.CLSCTX_INPROC_SERVER, IntPtr.Zero, out volObject);
            if (volObject is IAudioEndpointVolume)
            {
                _defaultDeviceAudioEndpointVolume = volObject as IAudioEndpointVolume;
                _defaultDeviceAudioEndpointVolume.RegisterControlChangeNotify(this);
            }
        }

        private void CheckVolumeState()
        {
            var scalar = GetVolume();
            if (scalar.HasValue)
            {
                VolumeChanged?.Invoke(scalar.Value);
            }
        }

        private void CheckMuteState()
        {
            var isMuted = GetMuted();
            if (isMuted.HasValue)
            {
                MuteChanged?.Invoke(isMuted.Value);
            }
        }

        public bool? GetMuted()
        {
            if (_defaultDeviceAudioEndpointVolume == null)
            {
                return null;
            }

            var isMuted = false;
            _defaultDeviceAudioEndpointVolume?.GetMute(out isMuted);
            return isMuted;
        }

        public float? GetVolume()
        {
            if(_defaultDeviceAudioEndpointVolume == null)
            {
                return null;
            }

            var scalar = 0f;
            _defaultDeviceAudioEndpointVolume?.GetMasterVolumeLevelScalar(out scalar);
            return scalar;
        }

        public int SetVolumeScalar(float value)
        {
            return _defaultDeviceAudioEndpointVolume.SetMasterVolumeLevelScalar(value, Guid.NewGuid());
        }

        public int? VolumeStepUp()
        {
            return _defaultDeviceAudioEndpointVolume?.VolumeStepUp(Guid.NewGuid());
        }

        public int? VolumeStepDown()
        {
            return _defaultDeviceAudioEndpointVolume?.VolumeStepDown(Guid.NewGuid());
        }

        public int SetMutedState(bool muted)
        {
            return _defaultDeviceAudioEndpointVolume.SetMute(muted, Guid.NewGuid());
        }
    }
}
