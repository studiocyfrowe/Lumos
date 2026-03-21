using Lumos.Agent.Collectors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ServiceProcess;
using System.Timers;
using Lumos.Agent.Application.Helpers;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Domain;

namespace Lumos.Agent
{
    public partial class Lumos : ServiceBase
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private bool _isRunning;

        public Lumos(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceName = "LumosMService";
            _serviceProvider = serviceProvider;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                SQLitePCL.Batteries.Init();

                using (var scope = _serviceProvider.CreateScope())
                {
                    scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();
                }

                _timer = new Timer(10_000);
                _timer.Elapsed += ExecuteTask;
                _timer.AutoReset = false;
                _timer.Start();

                FileLogger.Log("[STATUS] Lumos started");
            }
            catch (Exception ex)
            {
                FileLogger.Log($"[ERROR] Start error: {ex}");
                throw;
            }
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            _timer?.Dispose();

            FileLogger.Log("[STATUS] Lumos stopped");
        }

        public void DebugStart()
        {
            OnStart(null);
            Console.WriteLine("[LOG] running in DEBUG mode. Press ENTER to stop...");
            Console.ReadLine();
            OnStop();
        }

        private async void ExecuteTask(object sender, ElapsedEventArgs e)
        {
            if (_isRunning)
                return;

            _isRunning = true;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var memoryRepo = scope.ServiceProvider
                        .GetRequiredService<BaseRepositoryInterface<MemoryRAM>>();

                    var deviceRepo = scope.ServiceProvider
                        .GetRequiredService<BaseRepositoryInterface<DeviceInfo>>();

                    var processor = scope.ServiceProvider
                        .GetRequiredService<BaseRepositoryInterface<ProcessorCPU>>();

                    var device = DeviceInfoCollector.Collect();
                    var user = UserSessionCollector.GetLoggedUser();

                    await memoryRepo.InsertAsync(device);
                    await memoryRepo.DeleteAsync(device);

                    await processor.InsertAsync(device);
                    await processor.DeleteAsync(device);

                    FileLogger.Log($"[STATUS] Station data has been updated: {device.MachineName}");
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log($"[ERROR] Task error: {ex}");
            }
            finally
            {
                _isRunning = false;
                _timer?.Start();
            }
        }
    }
}