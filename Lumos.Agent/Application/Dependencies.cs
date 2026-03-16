using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lumos.Agent.Application
{
    public static class Dependencies
    {
        public static IServiceCollection AddAgentApplication(this IServiceCollection services)
        {
            services.AddScoped<IMemoryRAMManager, MemoryRAMManager>();
            return services;
        }
    }
}
