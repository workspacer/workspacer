using System;
using workspacer.Sound.Endpoints;
using Xunit;
using Vannatech.CoreAudio.Interfaces;
using Moq;
using Vannatech.CoreAudio.Enumerations;
using System.Linq;

namespace workspacer.Sound.Tests
{
    public class SoundManagerTests
    {
        /// <summary>
        /// Assess that a device is created for default
        /// </summary>
        [Fact]
        public void SoundManager_GetClientForDefault()
        {
            // Init
            var deviceId = Guid.NewGuid().ToString();
            var deviceType = EDataFlow.eAll;
            var roles = new[]
            {
                ERole.eCommunications
            };

            var deviceEnumeratorMock = new Mock<IMMDeviceEnumerator>();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            deviceMock.Setup(x => x.GetId(out deviceId));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            var device = deviceMock.Object;
            deviceEnumeratorMock.Setup(x => x.GetDefaultAudioEndpoint(It.Is<EDataFlow>(x => x == deviceType), It.IsIn(roles), out device));
            deviceEnumeratorMock.Setup(x => x.GetDevice(It.Is<string>(x => x == deviceId), out device));

            var manager = new SoundManager(deviceEnumeratorMock.Object);

            // Execute
            var result = manager.CreateClientForDefault(deviceType.ConvertToDeviceType(), roles.Select(x => x.ConvertToDeviceRole()).ToArray());

            //Assess
            Assert.Null(result.DeviceInfo.Id);
            Assert.Equal(deviceType.ConvertToDeviceType(), result.DeviceInfo.Type.Value);
            Assert.Equal(roles.Select(x => x.ConvertToDeviceRole()).ToArray(), result.DeviceInfo.Roles);

            deviceEnumeratorMock.VerifyAll();
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that
        /// </summary>
        [Fact]
        public void SoundManager_GetClientForDeviceId()
        {
            // Init
            var deviceId = Guid.NewGuid().ToString();

            var deviceEnumeratorMock = new Mock<IMMDeviceEnumerator>();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            var device = deviceMock.Object;
            deviceEnumeratorMock.Setup(x => x.GetDevice(It.Is<string>(x => x == deviceId), out device));

            var manager = new SoundManager(deviceEnumeratorMock.Object);

            // Execute
            var result = manager.CreateClientForDeviceId(deviceId);

            //Assess
            Assert.Equal(deviceId, result.DeviceInfo.Id);

            deviceEnumeratorMock.VerifyAll();
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that
        /// </summary>
        [Fact]
        public void SoundManager_GetClientsForDeviceId()
        {
            // Init
            var deviceId = Guid.NewGuid().ToString();

            var deviceEnumeratorMock = new Mock<IMMDeviceEnumerator>();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            var device = deviceMock.Object;
            deviceEnumeratorMock.Setup(x => x.GetDevice(It.Is<string>(x => x == deviceId), out device));

            var manager = new SoundManager(deviceEnumeratorMock.Object);

            // Execute
            manager.CreateClientForDeviceId(deviceId);
            var result = manager.GetClientsForDeviceId(deviceId);

            //Assess
            Assert.Equal(deviceId, result.First().DeviceInfo.Id);

            deviceEnumeratorMock.VerifyAll();
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that
        /// </summary>
        [Fact]
        public void SoundManager_GetDefaultPlaybackDeviceId()
        {
            // Init
            var deviceId = Guid.NewGuid().ToString();
            var deviceType = EDataFlow.eRender;
            var roles = new[]
            {
                ERole.eMultimedia
            };

            var deviceEnumeratorMock = new Mock<IMMDeviceEnumerator>();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            deviceMock.Setup(x => x.GetId(out deviceId));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;

            var device = deviceMock.Object;
            deviceEnumeratorMock.Setup(x => x.GetDefaultAudioEndpoint(It.Is<EDataFlow>(x => x == deviceType), It.IsIn(roles), out device));

            var manager = new SoundManager(deviceEnumeratorMock.Object);

            // Execute
            var result = manager.GetDefaultPlaybackDeviceId();

            //Assess
            Assert.Equal(deviceId, result);

            deviceEnumeratorMock.VerifyAll();
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that
        /// </summary>
        [Fact]
        public void SoundManager_GetDefaultRecordingDeviceId()
        {
            // Init
            var deviceId = Guid.NewGuid().ToString();
            var deviceType = EDataFlow.eCapture;
            var roles = new[]
            {
                ERole.eCommunications
            };

            var deviceEnumeratorMock = new Mock<IMMDeviceEnumerator>();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            deviceMock.Setup(x => x.GetId(out deviceId));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;

            var device = deviceMock.Object;
            deviceEnumeratorMock.Setup(x => x.GetDefaultAudioEndpoint(It.Is<EDataFlow>(x => x == deviceType), It.IsIn(roles), out device));

            var manager = new SoundManager(deviceEnumeratorMock.Object);

            // Execute
            var result = manager.GetDefaultRecordingDeviceId();

            //Assess
            Assert.Equal(deviceId, result);

            deviceEnumeratorMock.VerifyAll();
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }
    }
}
