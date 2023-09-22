using System;
using Vannatech.CoreAudio.Enumerations;

namespace workspacer.Sound
{
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
