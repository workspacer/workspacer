using System;
using System.Collections.Generic;
using System.Linq;
using Vannatech.CoreAudio.Constants;
using Vannatech.CoreAudio.Enumerations;
using Vannatech.CoreAudio.Externals;
using Vannatech.CoreAudio.Interfaces;

namespace workspacer.Sound
{
    public delegate void DeviceEventDelegate(DeviceInfo device);
    public delegate void DeviceDataEventDelegate<DataT>(DeviceInfo device, DataT newState);

    public class SoundManager : IMMNotificationClient
    {
        private IMMDeviceEnumerator _mmDeviceEnumerator;

        private List<SoundClient> _clients = new();

        public event DeviceEventDelegate DeviceRemoved;
        public event DeviceEventDelegate DeviceAdded;
        public event DeviceDataEventDelegate<PROPERTYKEY> DevicePropertyValueChanged;
        public event DeviceDataEventDelegate<uint> DeviceChanged;

        public event DeviceDataEventDelegate<float> VolumeChanged;
        public event DeviceDataEventDelegate<bool> MuteChanged;

        public SoundManager()
        {
            _mmDeviceEnumerator = Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComCLSIDs.MMDeviceEnumeratorCLSID))) as IMMDeviceEnumerator;
            if (_mmDeviceEnumerator == null)
            {
                throw new Exception("Creating device enumerator failed");
            }

            _mmDeviceEnumerator.RegisterEndpointNotificationCallback(this);
        }

        public SoundClient RegisterClientForDefault(DeviceType type, params DeviceRole[] roles)
        {
            var deviceId = GetDefaultDeviceId(type, roles);
            var client = RegisterClient(deviceId, type, roles);

            return client;
        }

        public SoundClient RegisterClientForDeviceId(string deviceId)
        {
            return RegisterClient(deviceId);
        }

        private SoundClient RegisterClient(string deviceId, DeviceType? type = null, params DeviceRole[] roles)
        {
            var client = CreateClient(deviceId, type, roles);

            //Map events
            client.VolumeChanged += HandleVolumeChanged;
            client.MuteChanged += HandleMuteChanged;

            _clients.Add(client);

            return client;
        }

        private void HandleMuteChanged(DeviceInfo device, bool newState)
        {
            MuteChanged?.Invoke(device, newState);
        }

        private void HandleVolumeChanged(DeviceInfo device, float newState)
        {
            VolumeChanged?.Invoke(device, newState);
        }

        public IEnumerable<SoundClient> GetClientsForDeviceId(string deviceId)
        {
            return _clients.Where(x => x.DeviceInfo.Id == deviceId);
        }

        public string GetDefaultPlaybackDeviceId(DeviceRole role = DeviceRole.Multimedia)
        {
            return GetDefaultDeviceId(DeviceType.Render, role);
        }

        public string GetDefaultRecordingDeviceId(DeviceRole role = DeviceRole.Communications)
        {
            return GetDefaultDeviceId(DeviceType.Capture, role);
        }

        public string GetDefaultDeviceId(DeviceType type, params DeviceRole[] roles)
        {
            if (_mmDeviceEnumerator == null)
            {
                return null;
            }

            IMMDevice device = null;
            foreach (var role in roles)
            {
                _mmDeviceEnumerator.GetDefaultAudioEndpoint(type.ConvertToEDataFlow(), role.ConvertToERole(), out device);
                if(device != null)
                {
                    break;
                }
            }

            var id = string.Empty;
            device?.GetId(out id);
            return id;
        }

        private SoundClient CreateClient(string deviceId, DeviceType? type = null, params DeviceRole[] roles)
        {
            if (_mmDeviceEnumerator == null)
            {
                return null;
            }

            _mmDeviceEnumerator.GetDevice(deviceId, out var device);
            if (device == null)
            {
                return null;
            }

            var client = new SoundClient();
            client.SetDevice(device);
            client.DeviceInfo = new DeviceInfo { Id = type.HasValue && roles.Any() ? null : deviceId, Type = type, Roles = roles };

            return client;
        }

        private void ApplyToClientsWithCriteria(string deviceId, DeviceType? type, DeviceRole? role, Action<SoundClient> action)
        {
            IEnumerable<SoundClient> clients = null;
            if (string.IsNullOrEmpty(deviceId))
            {
                clients = _clients.Where(x => x.DeviceInfo.Roles.Contains(role.Value) && x.DeviceInfo.Type == type.Value).ToList();
            }
            else
            {
                clients = _clients.Where(x => x.DeviceInfo.Id == deviceId).ToList();
            }

            foreach (var client in clients)
            {
                action(client);
            }
        }

        #region System Events

        public void OnDefaultDeviceChanged(EDataFlow type, ERole role, string defaultDeviceId)
        {
            ApplyToClientsWithCriteria(string.Empty, type.ConvertToDeviceType(), role.ConvertToDeviceRole(), (client) =>
            {
                _mmDeviceEnumerator.GetDevice(defaultDeviceId, out var device);
                if (device == null)
                {
                    return;
                }

                client.SetDevice(device);
                client.Refresh();
            });

#if EVENT_DEBUG
            MessageBox.Show($"DeviceRemoved DataFlow:[{dataFlow}] DeviceRole:[{deviceRole}] DeviceId:[{defaultDeviceId}]");
#endif
        }

        public void OnDeviceStateChanged(string deviceId, uint newState)
        {
            ApplyToClientsWithCriteria(deviceId, null, null, (client) => client.Refresh() );

            DeviceChanged?.Invoke(new DeviceInfo { Id = deviceId }, newState);
#if EVENT_DEBUG
            MessageBox.Show($"DeviceStateChanged deviceId:[{deviceId}] newState:[{newState}]");
#endif
        }

        public void OnDeviceAdded(string deviceId)
        {
            ApplyToClientsWithCriteria(deviceId, null, null, (client) => client.Refresh());

            DeviceAdded?.Invoke(new DeviceInfo { Id = deviceId });
#if EVENT_DEBUG
            MessageBox.Show($"DeviceAdded DeviceId:[{deviceId}]");
#endif
        }

        public void OnDeviceRemoved(string deviceId)
        {
            ApplyToClientsWithCriteria(deviceId, null, null, (client) => client.Refresh());

            DeviceRemoved?.Invoke(new DeviceInfo { Id = deviceId });
#if EVENT_DEBUG
            MessageBox.Show($"DeviceRemoved DeviceId:[{deviceId}]");
#endif
        }

        public void OnPropertyValueChanged(string deviceId, PROPERTYKEY propertyKey)
        {
            ApplyToClientsWithCriteria(deviceId, null, null, (client) => client.Refresh());

            DevicePropertyValueChanged?.Invoke(new DeviceInfo { Id = deviceId }, propertyKey);
#if EVENT_DEBUG
            MessageBox.Show($"DeviceStateChanged deviceId:[{deviceId}] propertyKey:[{propertyKey}]");
#endif
        }
        #endregion
    }

    public class SoundClient : IAudioEndpointVolumeCallback
    {
        public event DeviceDataEventDelegate<float> VolumeChanged;
        public event DeviceDataEventDelegate<bool> MuteChanged;

        private IMMDevice _device;

        public DeviceInfo DeviceInfo { get; set; }

        private IAudioEndpointVolume _deviceAudioEndpointVolume;

        public void SetDevice(IMMDevice device)
        {
            _device = device;
            InitializeAudioEnpoint(device);
        }

        private void InitializeAudioEnpoint(IMMDevice device = null)
        {
            device ??= _device;

            // Fetch IAudioEndpointVolume
            object volObject;
            device.Activate(new Guid(ComIIDs.IAudioEndpointVolumeIID), (uint)CLSCTX.CLSCTX_INPROC_SERVER, IntPtr.Zero, out volObject);
            if (volObject is IAudioEndpointVolume)
            {
                _deviceAudioEndpointVolume = volObject as IAudioEndpointVolume;
                _deviceAudioEndpointVolume.RegisterControlChangeNotify(this);
            }
        }

        //Notify on default device change
        public int OnNotify(IntPtr notificationData)
        {

#if EVENT_DEBUG
            MessageBox.Show($"OnNotify {notificationData}");
#endif
            if (_deviceAudioEndpointVolume == null)
            {
                return -1;
            }

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
            if (scalar.HasValue)
            {
                VolumeChanged?.Invoke(DeviceInfo, scalar.Value);
            }
        }

        private void UpdateMuted()
        {
            var isMuted = GetMuted();
            if (isMuted.HasValue)
            {
                MuteChanged?.Invoke(DeviceInfo, isMuted.Value);
            }
        }

        public bool? GetMuted()
        {
            if (_deviceAudioEndpointVolume == null)
            {
                return null;
            }

            var isMuted = false;
            _deviceAudioEndpointVolume?.GetMute(out isMuted);
            return isMuted;
        }

        public float? GetVolume()
        {
            if (_deviceAudioEndpointVolume == null)
            {
                return null;
            }

            var scalar = 0f;
            _deviceAudioEndpointVolume?.GetMasterVolumeLevelScalar(out scalar);
            return scalar;
        }

        public int? SetVolumeScalar(float value)
        {
            return _deviceAudioEndpointVolume?.SetMasterVolumeLevelScalar(value, Guid.NewGuid());
        }

        public int? VolumeStepUp()
        {
            return _deviceAudioEndpointVolume?.VolumeStepUp(Guid.NewGuid());
        }

        public int? VolumeStepDown()
        {
            return _deviceAudioEndpointVolume?.VolumeStepDown(Guid.NewGuid());
        }

        public int? SetMutedState(bool muted)
        {
            return _deviceAudioEndpointVolume?.SetMute(muted, Guid.NewGuid());
        }
    }

    public static class EnumConversionExtensions
    {
        public static ERole ConvertToERole(this DeviceRole role)
        {
            switch (role)
            {
                case DeviceRole.Console:
                    return ERole.eConsole;
                case DeviceRole.Communications:
                    return ERole.eCommunications;
                case DeviceRole.Multimedia:
                    return ERole.eMultimedia;
                default:
                    throw new Exception();
            }
        }

        public static DeviceRole ConvertToDeviceRole(this ERole role)
        {
            switch (role)
            {
                case ERole.eConsole:
                    return DeviceRole.Console;
                case ERole.eCommunications:
                    return DeviceRole.Communications;
                case ERole.eMultimedia:
                    return DeviceRole.Multimedia;
                default:
                    throw new Exception();
            }
        }

        public static EDataFlow ConvertToEDataFlow(this DeviceType eDataFlow)
        {
            switch (eDataFlow)
            {
                case DeviceType.Render:
                    return EDataFlow.eRender;
                case DeviceType.Capture:
                    return EDataFlow.eCapture;
                case DeviceType.All:
                    return EDataFlow.eAll;
                default:
                    throw new Exception();
            }
        }

        public static DeviceType ConvertToDeviceType(this EDataFlow eDataFlow)
        {
            switch (eDataFlow)
            {
                case EDataFlow.eRender:
                    return DeviceType.Render;
                case EDataFlow.eCapture:
                    return DeviceType.Capture;
                case EDataFlow.eAll:
                    return DeviceType.All;
                default:
                    throw new Exception();
            }
        }
    }
}
