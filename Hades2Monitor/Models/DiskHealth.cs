using Hades2Monitor.Common.Enums;

namespace Hades2Monitor.Models
{
    public class DiskHealth
    {
        public string DiskId { get; set; }
        public int OverallHealthPercent { get; set; }
        public int? WearLevelPercent { get; set; }
        public long? ReallocatedSectors { get; set; }
        public long? MediaErrors { get; set; }
        public long PowerOnHours { get; set; }
        public int TemperatureC { get; set; }
        public int UnsafeShutdownCount { get; set; }
        public DiskStatus Status { get; set; } = DiskStatus.OK;
    }
}
