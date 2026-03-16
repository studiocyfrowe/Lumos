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

                FileLogger.Log("Hades2Monitor started");
            }
            catch (Exception ex)
            {
                FileLogger.Log($"Start error: {ex}");
                throw;
            }
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            _timer?.Dispose();

            FileLogger.Log("Hades2Monitor stopped");
        }

        public void DebugStart()
        {
            OnStart(null);
            Console.WriteLine("Hades2Monitor running in DEBUG mode. Press ENTER to stop...");
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

                    var device = DeviceInfoCollector.Collect();
                    var user = UserSessionCollector.GetLoggedUser();

                    await memoryRepo.InsertAsync(device);

                    FileLogger.Log($"Updated station: {device.MachineName}");
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log($"Task error: {ex}");
            }
            finally
            {
                _isRunning = false;

                // 🔥 restart timera ręcznie (bo AutoReset = false)
                _timer?.Start();
            }
        }
    }
}