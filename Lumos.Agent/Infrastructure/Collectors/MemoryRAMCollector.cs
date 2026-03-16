using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Models;
using System;
using System.Management;

namespace Lumos.Agent.Collectors
{
    public class MemoryRAMCollector : BaseCollectorInterface<MemoryRAM>
    {
        public MemoryRAM Collect()
        {
            var searcher = new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

            foreach (ManagementObject os in searcher.Get())
            {
                // wartości WMI są w KB
                double totalKB = Convert.ToDouble(os["TotalVisibleMemorySize"]);
                double freeKB = Convert.ToDouble(os["FreePhysicalMemory"]);

                double totalGB = Math.Round(totalKB / 1024 / 1024, 2);
                double freeGB = Math.Round(freeKB / 1024 / 1024, 2);
                double usedGB = Math.Round(totalGB - freeGB, 2);
                double usedPercent = Math.Round((usedGB / totalGB) * 100, 1);

                return new MemoryRAM
                {
                    TotalGB = totalGB,
                    FreeGB = freeGB,
                    UsedGB = usedGB,
                    UsedPercent = usedPercent
                };
            }

            throw new InvalidOperationException("Nie udało się pobrać danych RAM z WMI.");
        }
    }
}
