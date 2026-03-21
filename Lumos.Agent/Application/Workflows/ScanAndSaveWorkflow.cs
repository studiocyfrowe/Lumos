using Lumos.Agent.Application.Helpers;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Domain;
using Lumos.Agent.Infrastructure.Interfaces;
using System;
using System.Timers;

namespace Lumos.Agent.Application.Workflows
{
    public class ScanAndSaveWorkflow : BaseWorkflowInterface
    {
        private readonly BaseCollectorInterface<DeviceInfo> _deviceInfoCollector;   
        private readonly BaseRepositoryInterface<MemoryRAM> _memoryRepository;
        private readonly BaseRepositoryInterface<DeviceInfo> _deviceRepository;
        private readonly BaseRepositoryInterface<ProcessorCPU> _processorRepository;

        public ScanAndSaveWorkflow(
            BaseCollectorInterface<DeviceInfo> deviceInfoCollector,
            BaseRepositoryInterface<MemoryRAM> memoryRepository,
            BaseRepositoryInterface<DeviceInfo> deviceRepository,
            BaseRepositoryInterface<ProcessorCPU> processorRepository)
        {
            _deviceInfoCollector = deviceInfoCollector;
            _memoryRepository = memoryRepository;
            _deviceRepository = deviceRepository;
            _processorRepository = processorRepository;
        }

        public async void ExecuteTask()
        {
            try
            {
                var device = _deviceInfoCollector.Collect();
                FileLogger.Log($"[STATUS] Device data: {device.MachineName} has been collected");

                await _memoryRepository.InsertAsync(device);
                await _memoryRepository.DeleteAsync(device);
                FileLogger.Log($"[STATUS] Memory RAM data has been updated");

                await _processorRepository.InsertAsync(device);
                await _processorRepository.DeleteAsync(device);
                FileLogger.Log($"[STATUS] Processor CPU data has been updated");

                FileLogger.Log($"[STATUS] Station data has been updated");
            }
            catch (Exception ex)
            {
                FileLogger.Log($"[ERROR] Task error: {ex}");
            }
        }
    }
}
