namespace Lumos.Agent.Models
{
    public class ProcessorCPU
    {
        public string Name { get; set; }
        public int NumberOfCores { get; set; }
        public int NumberOfLogicalProcessors { get; set; }
        public int MaxClockSpeedMHz { get; set; }
        public int LoadPercent { get; set; }
    }
}
