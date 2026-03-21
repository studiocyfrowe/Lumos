using Lumos.Agent.Application.Contexts;
using Lumos.Agent.Application.Helpers;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Collectors;
using Lumos.Agent.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace Lumos.Agent
{
    public partial class Lumos : ServiceBase
    {
        private System.Timers.Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private bool _isRunning;

        public Lumos(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceName = "LumosAgent";
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
                        .GetRequiredService<LumosContext>();
                }

                _timer = new System.Timers.Timer(10_000);
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

        private int _isRunningFlag = 0;

        private void ExecuteTask(object sender, ElapsedEventArgs e)
        {
            _timer?.Stop();

            if (Interlocked.Exchange(ref _isRunningFlag, 1) == 1)
                return;

            try
            {
                FileLogger.Log("[STATUS] Starting device scan");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var scanWorkflow = scope.ServiceProvider
                        .GetRequiredService<BaseWorkflowInterface>();

                    scanWorkflow.ExecuteTask();
                }

                FileLogger.Log("[STATUS] Scanning the device has been finished");
            }
            catch (Exception ex)
            {
                FileLogger.Log($"[ERROR] Scan error: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                Interlocked.Exchange(ref _isRunningFlag, 0);
                _timer?.Start();
            }
        }
    }
}