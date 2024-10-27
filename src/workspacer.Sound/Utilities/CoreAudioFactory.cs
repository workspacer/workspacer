using System;
using Vannatech.CoreAudio.Constants;
using Vannatech.CoreAudio.Interfaces;

namespace workspacer.Sound
{
    public static class CoreAudioFactory
    {
        public static Type DeviceEnumType = Type.GetTypeFromCLSID(new Guid(ComCLSIDs.MMDeviceEnumeratorCLSID));
        public static IMMDeviceEnumerator CreateDeviceEnumerator()
        {
            return Activator.CreateInstance(DeviceEnumType) as IMMDeviceEnumerator;
        }
    }
}
