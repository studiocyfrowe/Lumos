using Hades2Monitor.Collectors;
using Hades2Monitor.Models;
using Hades2Monitor.Repositories.Interfaces;
using Hades2Monitor.Helpers;
using System;
using System.ServiceProcess;
using System.Timers;

namespace Hades2Monitor
{
    public partial class Hades2Monitor : ServiceBase
    {
        private Timer _timer;
        private readonly BaseRepositoryInterface<MemoryRAM> _memoryRAMRepository;
        private readonly BaseRepositoryInterface<DeviceInfo> _deviceInfoRepository;
        private readonly BaseRepositoryInterface<ProcessorCPU> _processorCPURepository;
        private bool _isRunning;
        private IServiceProvider serviceProvider;

        public Hades2Monitor(
            IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceName = "Hades2Monitor";
            this.serviceProvider = serviceProvider;
            _memoryRAMRepository = (BaseRepositoryInterface<MemoryRAM>)serviceProvider.GetService(typeof(BaseRepositoryInterface<MemoryRAM>));
            _deviceInfoRepository = (BaseRepositoryInterface<DeviceInfo>)serviceProvider.GetService(typeof(BaseRepositoryInterface<DeviceInfo>));
            _processorCPURepository = (BaseRepositoryInterface<ProcessorCPU>)serviceProvider.GetService(typeof(BaseRepositoryInterface<ProcessorCPU>));

            if (_memoryRAMRepository == null || _deviceInfoRepository == null || _processorCPURepository == null)
                FileLogger.Log("Error: Repositories could not be resolved from service provider.");
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _timer = new Timer(10_000); // 10 sekund
                _timer.Elapsed += ExecuteTask;
                _timer.AutoReset = true;
                _timer.Start();

                Logger.Info("Hades2Monitor started");
                FileLogger.Log("Hades2Monitor started");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                FileLogger.Log($"Error: {ex.Message}");
            }
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            _timer?.Dispose();

            Logger.Info("Hades2Monitor stopped");
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

            try
            {
                _isRunning = true;

                var device = DeviceInfoCollector.Collect();
                var user = UserSessionCollector.GetLoggedUser();

                await _memoryRAMRepository.InsertAsync(device);
                await _memoryRAMRepository.DeleteAsync(device);

                await _processorCPURepository.InsertAsync(device);
                //await _deviceInfoRepository.UpsertAsync(device, user);

                FileLogger.Log($"Zaktualizowano dane o stacji: {device.MachineName}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                FileLogger.Log($"Error: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }
    }
}
