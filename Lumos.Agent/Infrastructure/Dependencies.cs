using Lumos.Agent.Domain;
using Lumos.Agent.Infrastructure.Factories;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lumos.Agent.Infrastructure
{
    public static class Dependencies
    {
        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton(DatabaseConfig.GetConnectionString());

            services.AddSingleton<IQueryHelperFactory, QueryHelperFactory>();

            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<BaseRepositoryInterface<MemoryRAM>, MemoryRAMRepository>();
            services.AddScoped<BaseRepositoryInterface<DeviceInfo>, DeviceInfoRepository>();
            services.AddScoped<BaseRepositoryInterface<ProcessorCPU>, ProcessorCPURepository>();

            return services;
        }
    }
}
