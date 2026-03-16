namespace Lumos.Agent.Models
{
    public class WorkStation
    {
        public string MachineGuid { get; set; }
        public string MachineName { get; set; }
        public string OSVersion { get; set; }
        public string LoggedUser { get; set; }
        public string LoggedUserDomain { get; set; }
    }
}
