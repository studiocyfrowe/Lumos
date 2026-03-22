using Lumos.Agent.Application.Contexts;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Domain;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Infrastructure.Providers;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace Lumos.Agent.Application.Repositories
{
    public class ProcessorCPURepository : BaseRepositoryInterface<ProcessorCPU>
    {
        private readonly LumosContext _context;
        private DeviceIdentityProvider identityProvider { get; set; }
        private readonly BaseCollectorInterface<ProcessorCPU> _processorCPUCollector;

        public ProcessorCPURepository(
            LumosContext context, 
            DeviceIdentityProvider identityProvider, 
            BaseCollectorInterface<ProcessorCPU> processorCPUCollector)
        {
            _context = context;
            this.identityProvider = identityProvider;
            _processorCPUCollector = processorCPUCollector;
        }

        public async Task InsertAsync(object entity)
        {
            if (!(entity is DeviceInfo))
                throw new ArgumentException(
                    "MemoryRamScanRepository expects DeviceInfo as entity",
                    nameof(entity)
                );

            var device = (DeviceInfo)entity;

            var machineGuid = this.identityProvider.GetMachineGuid();
            var processor = _processorCPUCollector.Collect();

            const string sql = @"
                INSERT INTO ProcessorCPUs (MachineGuid, Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeedMHz, LoadPercent, DeviceName)
                VALUES (@MachineGuid, @Name, @NumberOfCores, @NumberOfLogicalProcessors, @MaxClockSpeedMHz, @LoadPercent, @DeviceName);
            ";

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqliteParameter("@MachineGuid", machineGuid.ToString()),
                new SqliteParameter("@Name", processor.Name),
                new SqliteParameter("@NumberOfCores", processor.NumberOfCores),
                new SqliteParameter("@NumberOfLogicalProcessors", processor.NumberOfLogicalProcessors),
                new SqliteParameter("@MaxClockSpeedMHz", processor.MaxClockSpeedMHz),
                new SqliteParameter("@LoadPercent", processor.LoadPercent),
                new SqliteParameter("@DeviceName", device.MachineName)
            );
        }

        public async Task DeleteAsync(object value)
        {
            string sql = @"
               WITH Ranked AS
                (
                    SELECT
                        Id,
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY MachineGuid
                            ORDER BY LastScan DESC
                        ) AS rn
                    FROM ProcessorCPUs
                    WHERE MachineGuid = @MachineGuid
                )
                DELETE FROM ProcessorCPUs
                WHERE Id IN
                (
                    SELECT Id
                    FROM Ranked
                    WHERE rn > 60
                );
            ";

            var MachineGuid = this.identityProvider.GetMachineGuid();

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqliteParameter("@MachineGuid", MachineGuid.ToString())
            );
        }
    }
}
