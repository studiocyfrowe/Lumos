namespace Lumos.Agent.Domain
{
    public sealed class DeviceInfo
    {
        public string MachineGuid { get; set; }
        public string MachineName { get; set; }
        public string OSVersion { get; set; }
        public int ProcessorCount { get; set; }
        public bool Is64Bit { get; set; }
    }
}
