using Hades2Monitor.Models;
using System;

namespace Hades2Monitor.Collectors
{
    public class DeviceInfoCollector
    {
        public static DeviceInfo Collect()
        {
            return new DeviceInfo
            {
                MachineGuid = Providers.DeviceIdentityProvider.GetMachineGuid().ToString(),
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                Is64Bit = Environment.Is64BitOperatingSystem
            };
        }
    }
}
