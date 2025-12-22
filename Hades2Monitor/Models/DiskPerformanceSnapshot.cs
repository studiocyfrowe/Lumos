using System;

namespace Hades2Monitor.Models
{
    public class DiskPerformanceSnapshot
    {
        public string DiskId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public double AvgReadLatencyMs { get; set; }
        public double AvgWriteLatencyMs { get; set; }
        public double ReadIOPS { get; set; }
        public double WriteIOPS { get; set; }
        public double ThroughputMBs { get; set; }
        public double QueueLength { get; set; }
    }
}
