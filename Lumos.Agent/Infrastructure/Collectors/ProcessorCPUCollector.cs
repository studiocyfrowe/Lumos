using Lumos.Agent.Domain;
using Lumos.Agent.Infrastructure.Interfaces;
using System;
using System.Management;

namespace Lumos.Agent.Infrastructure.Collectors
{
    public class ProcessorCPUCollector : BaseCollectorInterface<ProcessorCPU>
    {
        public ProcessorCPUCollector() { }
        public ProcessorCPU Collect()
        {
            var searcher = new ManagementObjectSearcher(
                "SELECT Name, NumberOfCores, NumberOfLogicalProcessors, LoadPercentage, MaxClockSpeed FROM Win32_Processor");

            foreach (ManagementObject cpu in searcher.Get())
            {
                string name = cpu["Name"]?.ToString() ?? "Unknown";
                int cores = Convert.ToInt32(cpu["NumberOfCores"]);
                int logicalProcessors = Convert.ToInt32(cpu["NumberOfLogicalProcessors"]);
                int maxClock = Convert.ToInt32(cpu["MaxClockSpeed"]);
                int load = Convert.ToInt32(cpu["LoadPercentage"]); // aktualne obciążenie CPU w %

                return new ProcessorCPU
                {
                    Name = name,
                    NumberOfCores = cores,
                    NumberOfLogicalProcessors = logicalProcessors,
                    MaxClockSpeedMHz = maxClock,
                    LoadPercent = load
                };
            }

            throw new InvalidOperationException("Nie udało się pobrać danych CPU z WMI.");
        }
    }
}
