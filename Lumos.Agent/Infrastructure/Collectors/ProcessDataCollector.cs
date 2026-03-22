using Lumos.Agent.Domain.Models;
using Lumos.Agent.Infrastructure.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;

namespace Lumos.Agent.Infrastructure.Collectors
{
    public class ProcessDataCollector : BaseCollectorInterface<ProcessModel>
    {
        public ProcessModel Collect()
        {
            var processes = Process.GetProcesses();

            var result = new ProcessModel
            {
                Processes = processes.Select(p =>
                {
                    try
                    {
                        return new ProcessItem
                        {
                            MachineGuid = Environment.MachineName,
                            ProcessName = p.ProcessName,
                            ExecutablePath = p.MainModule?.FileName ?? string.Empty,
                            StartTime = p.StartTime.ToString("o"), // ISO 8601 format
                            MemoryUsageMB = p.WorkingSet64 / (1024 * 1024),
                            ProcessId = p.Id.ToString(),
                            CpuUsagePercent = 0
                        };
                    }
                    catch
                    {
                        return null; 
                    }
                })
                .Where(p => p != null)
                .ToList()
            };

            return result;
        }
    }
}
