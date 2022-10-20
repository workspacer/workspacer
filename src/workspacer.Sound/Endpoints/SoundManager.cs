using System;
using System.Collections.Generic;
using System.Linq;
using Vannatech.CoreAudio.Enumerations;
using Vannatech.CoreAudio.Externals;
using Vannatech.CoreAudio.Interfaces;
using workspacer.Sound.Exceptions;

namespace workspacer.Sound.Endpoints
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

        public SoundManager(IMMDeviceEnumerator deviceEnumerator)
        {
            _mmDeviceEnumerator = deviceEnumerator;
            _mmDeviceEnumerator.RegisterEndpointNotificationCallback(this);
        }

        public SoundClient CreateClientForDefault(DeviceType type, params DeviceRole[] roles)
        {
            var deviceId = GetDefaultDeviceId(type, roles);
            var client = CreateClient(deviceId, type, roles);

            return client;
        }

        public SoundClient CreateClientForDeviceId(string deviceId)
        {
            return CreateClient(deviceId);
        }

        private SoundClient CreateClient(string deviceId, DeviceType? type = null, params DeviceRole[] roles)
        {
            _mmDeviceEnumerator.GetDevice(deviceId, out var device);
            if (device == null)
            {
                throw new DeviceNotFoundException();
            }

            var client = new SoundClient();
            client.SetDevice(device);
            client.DeviceInfo = new DeviceInfo { Id = type.HasValue && roles.Any() ? null : deviceId, Type = type, Roles = roles };

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
            var result = _clients.Where(x => x.DeviceInfo.Id == deviceId);
            if (result == null || !result.Any())
            {
                throw new ClientNotFoundException();
            }

            return result;
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
            foreach (var role in roles)
            {
                _mmDeviceEnumerator.GetDefaultAudioEndpoint(type.ConvertToEDataFlow(), role.ConvertToERole(), out var device);
                if (device != null)
                {
                    var id = string.Empty;
                    device?.GetId(out id);
                    return id;
                }
            }

            throw new DeviceNotFoundException();
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

            if (clients == null || !clients.Any())
            {
                throw new ClientNotFoundException();
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
                    throw new DeviceNotFoundException();
                }

                client.SetDevice(device);
                client.Refresh();
            });
        }

        public void OnDeviceStateChanged(string deviceId, uint newState)
        {
            ApplyToClientsWithCriteria(deviceId, null, null, (client) => client.Refresh());

            DeviceChanged?.Invoke(new DeviceInfo { Id = deviceId }, newState);
        }

        public void OnDeviceAdded(string deviceId)
        {
            ApplyToClientsWithCriteria(deviceId, null, null, (client) => client.Refresh());

            DeviceAdded?.Invoke(new DeviceInfo { Id = deviceId });
        }

        public void OnDeviceRemoved(string deviceId)
        {
            ApplyToClientsWithCriteria(deviceId, null, null, (client) => client.Refresh());

            DeviceRemoved?.Invoke(new DeviceInfo { Id = deviceId });
        }

        public void OnPropertyValueChanged(string deviceId, PROPERTYKEY propertyKey)
        {
            DevicePropertyValueChanged?.Invoke(new DeviceInfo { Id = deviceId }, propertyKey);
        }
        #endregion
    }
}
