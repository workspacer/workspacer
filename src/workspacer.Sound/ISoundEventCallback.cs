namespace workspacer.Sound
{
    public interface ISoundEventCallback
    {
        void VolumeChanged(float scalarVolume);
        void MuteChanged(bool isMuted);
        void DeviceChanged();
    }
}
