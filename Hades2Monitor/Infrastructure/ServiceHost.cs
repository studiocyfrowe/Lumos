using Hades2Monitor.Contexts.Interfaces;
using Hades2Monitor.Models;
using Hades2Monitor.Repositories;
using Hades2Monitor.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hades2Monitor.Infrastructure
{
    public class ServiceHost
    {
        public static IServiceProvider Build()
        {
            var services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton<Factories.IQueryHelperFactory, Factories.QueryHelperFactory>();

            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<BaseRepositoryInterface<MemoryRAM>, MemoryRAMRepository>();
            services.AddScoped<BaseRepositoryInterface<DeviceInfo>, DeviceInfoRepository>();
            services.AddScoped<BaseRepositoryInterface<ProcessorCPU>, ProcessorCPURepository>();

            return services.BuildServiceProvider();
        }
    }
}
