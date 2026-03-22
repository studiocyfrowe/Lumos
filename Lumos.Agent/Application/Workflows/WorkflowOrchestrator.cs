using Lumos.Agent.Application.Helpers;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Domain;
using Lumos.Agent.Domain.Models;
using Lumos.Agent.Infrastructure.Interfaces;
using System;

namespace Lumos.Agent.Application.Workflows
{
    public class WorkflowOrchestrator : BaseWorkflowInterface
    {
        private readonly BaseCollectorInterface<DeviceInfo> _deviceInfoCollector;
        private readonly BaseCollectorInterface<ProcessModel> _processCollector;
        private readonly BaseRepositoryInterface<MemoryRAM> _memoryRepository;
        private readonly BaseRepositoryInterface<ProcessorCPU> _processorRepository;
        private readonly BaseRepositoryInterface<ProcessModel> _processRepository;

        public WorkflowOrchestrator(
            BaseCollectorInterface<DeviceInfo> deviceInfoCollector,
            BaseCollectorInterface<ProcessModel> processCollector,
            BaseRepositoryInterface<MemoryRAM> memoryRepository,
            BaseRepositoryInterface<ProcessorCPU> processorRepository,
            BaseRepositoryInterface<ProcessModel> processRepository)
        {
            _deviceInfoCollector = deviceInfoCollector;
            _processCollector = processCollector;
            _memoryRepository = memoryRepository;
            _processorRepository = processorRepository;
            _processRepository = processRepository;
        }

        public async void ExecuteTask()
        {
            try
            {
                var device = _deviceInfoCollector.Collect();
                FileLogger.Log($"[STATUS] Device data: {device.MachineName} has been collected");

                var processes = _processCollector.Collect();
                FileLogger.Log($"[STATUS] Processes data: {device.MachineName} has been collected");

                await _memoryRepository.InsertAsync(device);
                await _memoryRepository.DeleteAsync(device);
                FileLogger.Log($"[STATUS] Memory RAM data has been updated");

                await _processorRepository.InsertAsync(device);
                await _processorRepository.DeleteAsync(device);
                FileLogger.Log($"[STATUS] Processor CPU data has been updated");

                await _processRepository.InsertAsync(processes);
                await _processRepository.DeleteAsync(processes);
                FileLogger.Log($"[STATUS] Processes data has been updated");

                FileLogger.Log($"[STATUS] Station data has been updated");
            }
            catch (Exception ex)
            {
                FileLogger.Log($"[ERROR] Task error: {ex}");
            }
        }
    }
}
