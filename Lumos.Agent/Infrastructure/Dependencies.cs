using Lumos.Agent.Collectors;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lumos.Agent.Infrastructure
{
    public static class Dependencies
    {
        public static IServiceCollection AddAgentInfrastructure(this IServiceCollection services)
        {
            //services.AddScoped<BaseCollectorInterface<DeviceInfo>, DeviceInfoCollector>();

            return services;
        }
    }
}
