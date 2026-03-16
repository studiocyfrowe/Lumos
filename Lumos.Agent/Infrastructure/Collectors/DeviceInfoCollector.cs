using Lumos.Agent.Domain.Providers;
using Lumos.Agent.Models;
using System;

namespace Lumos.Agent.Collectors
{
    public class DeviceInfoCollector
    {
        public static DeviceInfo Collect()
        {
            return new DeviceInfo
            {
                MachineGuid = DeviceIdentityProvider.GetMachineGuid().ToString(),
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                Is64Bit = Environment.Is64BitOperatingSystem
            };
        }
    }
}
