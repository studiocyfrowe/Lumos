using Lumos.Agent.Common.Enums;
using System;

namespace Lumos.Agent.Models
{
    public class DiskInfo
    {
        public string DiskId { get; set; } = Guid.NewGuid().ToString();
        public string Model { get; set; } = string.Empty;
        public string MachineGuid { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public DiskTypeEnum DiskType { get; set; } = DiskTypeEnum.Unknown;
        public DiskInterface Interface { get; set; } = DiskInterface.Unknown;
        public long CapacityGB { get; set; }
        public string FirmwareVersion { get; set; } = string.Empty;
        public string SerialHash { get; set; } = string.Empty;
        public bool IsSystemDisk { get; set; }
        public bool IsRemovable { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
