using Lumos.Agent.Domain;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Infrastructure.Providers;
using System;

namespace Lumos.Agent.Collectors
{
    public class DeviceInfoCollector : BaseCollectorInterface<DeviceInfo>
    {
        private DeviceIdentityProvider deviceIdentityProvider { get; set; }

        public DeviceInfoCollector(DeviceIdentityProvider deviceIdentityProvider)
        {
            this.deviceIdentityProvider = deviceIdentityProvider;
        }

        public DeviceInfo Collect()
        {
            return new DeviceInfo
            {
                MachineGuid = this.deviceIdentityProvider.GetMachineGuid().ToString(),
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                Is64Bit = Environment.Is64BitOperatingSystem
            };
        }
    }
}
