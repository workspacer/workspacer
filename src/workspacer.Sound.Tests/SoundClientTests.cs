using System;
using workspacer.Sound.Endpoints;
using Xunit;
using Moq;
using Vannatech.CoreAudio.Interfaces;

namespace workspacer.Sound.Tests
{
    public class SoundClientTests
    {
        /// <summary>
        /// Assess that set device with empty device doesn't crash
        /// </summary>
        [Fact]
        public void SoundClient_SetDevice_Empty()
        {
            var client = new SoundClient();
            IMMDevice device = null;

            Assert.Throws<Exception>(() => client.SetDevice(device));
        }

        /// <summary>
        /// Assess that SetDevice without endpoint works
        /// </summary>
        [Fact]
        public void SoundClient_SetDevice_NoEndpointCreated()
        {
            // Init
            var client = new SoundClient();
            var deviceMock = new Mock<IMMDevice>();
            object audioEndpointVolume = null;

            // Setup
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            // Execute
            Assert.Throws<Exception>(() => client.SetDevice(deviceMock.Object));

            // Assess
            deviceMock.VerifyAll();
        }

        /// <summary>
        /// Assess that SetDevice with endpoint registers
        /// </summary>
        [Fact]
        public void SoundClient_SetDevice_EndpointCreated()
        {
            // Init
            var client = new SoundClient();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            // Execute
            client.SetDevice(deviceMock.Object);

            // Assess
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that Refresh updates states
        /// </summary>
        [Fact]
        public void SoundClient_Refresh_UpdatesState()
        {
            // Init
            var client = new SoundClient();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();
            var volume = 1f;
            var mute = false;
            bool volumeEventFired = false;
            bool muteEventFired = false;

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            audioEndpointVolumeMock.Setup(x => x.GetMasterVolumeLevelScalar(out volume));
            audioEndpointVolumeMock.Setup(x => x.GetMute(out mute));

            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            client.VolumeChanged += (send, e) => { volumeEventFired = true; };
            client.MuteChanged += (send, e) => { muteEventFired = true; };

            // Execute
            client.SetDevice(deviceMock.Object);
            client.Refresh();

            // Assess
            Assert.True(volumeEventFired);
            Assert.True(muteEventFired);
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that volume state can be get
        /// </summary>
        [Fact]
        public void SoundClient_GetVolume_Returns()
        {
            // Init
            var client = new SoundClient();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();
            var volume = 0.5f;

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            audioEndpointVolumeMock.Setup(x => x.GetMasterVolumeLevelScalar(out volume));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            // Execute
            client.SetDevice(deviceMock.Object);
            var result = client.GetVolume();

            // Assess
            Assert.Equal(volume, result);
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that mute state can be get
        /// </summary>
        [Fact]
        public void SoundClient_GetMuted_Returns()
        {
            // Init
            var client = new SoundClient();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();
            var mute = true;

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            audioEndpointVolumeMock.Setup(x => x.GetMute(out mute));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            // Execute
            client.SetDevice(deviceMock.Object);
            var result = client.GetMuted();

            // Assess
            Assert.Equal(mute, result);
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that VolumeStepDown calls endpoint
        /// </summary>
        [Fact]
        public void SoundClient_VolumeStepDown()
        {
            // Init
            var client = new SoundClient();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            audioEndpointVolumeMock.Setup(x => x.VolumeStepDown(It.IsAny<Guid>()));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            // Execute
            client.SetDevice(deviceMock.Object);
            client.VolumeStepDown();

            // Assess
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }

        /// <summary>
        /// Assess that VolumeStepUp calls endpoint
        /// </summary>
        [Fact]
        public void SoundClient_VolumeStepUp()
        {
            // Init
            var client = new SoundClient();
            var deviceMock = new Mock<IMMDevice>();
            var audioEndpointVolumeMock = new Mock<IAudioEndpointVolume>();

            // Setup
            audioEndpointVolumeMock.Setup(x => x.RegisterControlChangeNotify(It.IsAny<IAudioEndpointVolumeCallback>()));
            audioEndpointVolumeMock.Setup(x => x.VolumeStepUp(It.IsAny<Guid>()));
            object audioEndpointVolume = audioEndpointVolumeMock.Object;
            deviceMock.Setup(x => x.Activate(It.IsAny<Guid>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), out audioEndpointVolume));

            // Execute
            client.SetDevice(deviceMock.Object);
            client.VolumeStepUp();

            // Assess
            deviceMock.VerifyAll();
            audioEndpointVolumeMock.VerifyAll();
        }
    }
}
