using Lumos.Agent.Application.Contexts;
using Lumos.Agent.Application.Helpers;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Application.Repositories;
using Lumos.Agent.Application.Workflows;
using Lumos.Agent.Domain;
using Lumos.Agent.Domain.Models;
using Lumos.Agent.Infrastructure;
using Lumos.Agent.Infrastructure.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace Lumos.Agent.Application
{
    public static class Dependencies
    {
        public static IServiceCollection AddAgentApplication(this IServiceCollection services)
        {
            services.AddSingleton(DatabaseConfig.GetConnectionString());

            services.AddSingleton<IQueryHelperFactory, QueryHelperFactory>();

            services.AddScoped<LumosContext>();
            services.AddScoped<SqlLoader>();

            services.AddScoped<BaseRepositoryInterface<MemoryRAM>, MemoryRAMRepository>();
            services.AddScoped<BaseRepositoryInterface<DeviceInfo>, DeviceInfoRepository>();
            services.AddScoped<BaseRepositoryInterface<ProcessorCPU>, ProcessorCPURepository>();
            services.AddScoped<BaseRepositoryInterface<ProcessModel>, ProcessRepository>();

            services.AddScoped<BaseWorkflowInterface, WorkflowOrchestrator>();

            return services;
        }
    }
}
