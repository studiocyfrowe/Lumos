using Lumos.Agent.Application;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Application.Services;
using Lumos.Agent.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Lumos.Agent
{
    public static class Dependencies
    {
        public static ServiceProvider Build()
        {
            var services = new ServiceCollection();

            services.AddAgentApplication();
            services.AddCommonInfrastructure();

            return services.BuildServiceProvider();
        }
    }
}
