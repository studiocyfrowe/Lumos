using System.ServiceProcess;

namespace Lumos.Agent
{
    internal static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// W trybie DEBUG uruchamia usługę jako konsolę.
        /// W trybie RELEASE uruchamia jako prawdziwą usługę Windows.
        /// </summary>
        static void Main()
        {
            // Budujemy kontener DI
            var serviceProvider = Dependencies.Build();

#if DEBUG
            // Tryb debug – uruchamiamy usługę w konsoli
            var service = new Lumos(serviceProvider);
            service.DebugStart(); // metoda symulująca OnStart/OnStop w konsoli
#else
            // Tryb produkcyjny – uruchamiamy Windows Service
            ServiceBase[] ServicesToRun = new ServiceBase[]
            {
                new Lumos(serviceProvider)
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
