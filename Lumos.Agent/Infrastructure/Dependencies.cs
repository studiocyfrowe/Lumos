using Lumos.Agent.Collectors;
using Lumos.Agent.Domain;
using Lumos.Agent.Domain.Models;
using Lumos.Agent.Infrastructure.Collectors;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Infrastructure.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Lumos.Agent.Infrastructure
{
    public static class Dependencies
    {
        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<BaseCollectorInterface<DeviceInfo>, DeviceInfoCollector>();
            services.AddScoped<BaseCollectorInterface<MemoryRAM>, MemoryRAMCollector>();
            services.AddScoped<BaseCollectorInterface<ProcessorCPU>, ProcessorCPUCollector>();
            services.AddScoped<BaseCollectorInterface<DiskInfo>, DiskInfoCollector>();
            services.AddScoped<BaseCollectorInterface<ProcessModel>, ProcessDataCollector>();

            services.AddScoped<DeviceIdentityProvider>();

            return services;
        }
    }
}
