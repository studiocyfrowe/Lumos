using Microsoft.Win32;
using System;

namespace Lumos.Agent.Infrastructure.Providers
{
    public class DeviceIdentityProvider
    {
        public DeviceIdentityProvider()
        {
        }

        public Guid GetMachineGuid()
        {
            using (var localMachineX64View =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                var key = localMachineX64View.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                var guid = key.GetValue("MachineGuid").ToString();
                return Guid.Parse(guid);
            }
        }
    }
}
