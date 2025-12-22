using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Hades2Monitor
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;

            serviceInstaller1.ServiceName = "Hades2Monitor";
            serviceInstaller1.DisplayName = "HaDes2 Monitor";
            serviceInstaller1.Description = "Moduł generowania raportu o dostępności stacji roboczych w domenie";
            serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
        }
    }
}
