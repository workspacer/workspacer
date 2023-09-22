using System.Collections.Generic;

namespace workspacer.Sound
{
    public class DeviceInfo
    {
        public string Id { get; set; }
        public ICollection<DeviceRole> Roles { get; set; } = new DeviceRole[0];
        public DeviceType? Type { get; set; }
    }

    public enum DeviceType
    {
        Render = 0,
        Capture = 1,
        All = 2
    }

    public enum DeviceRole
    {
        Console = 0,
        Multimedia = 1,
        Communications = 2
    }
}
