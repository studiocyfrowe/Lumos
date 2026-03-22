namespace Lumos.Agent.Domain.Models
{
    public class ProcessItem
    {
        public string MachineGuid { get; set; } = string.Empty;
        public string ProcessId { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public long MemoryUsageMB { get; set; }
        public string StartTime { get; set; }
        public double CpuUsagePercent { get; set; }
    }
}
