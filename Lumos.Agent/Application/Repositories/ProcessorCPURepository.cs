using Lumos.Agent.Domain;
using Lumos.Agent.Domain.Providers;
using Lumos.Agent.Infrastructure.Collectors;
using Lumos.Agent.Infrastructure.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace Lumos.Agent.Repositories
{
    public class ProcessorCPURepository : BaseRepositoryInterface<ProcessorCPU>
    {
        private readonly ApplicationDbContext _context;
        public ProcessorCPURepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InsertAsync(object entity)
        {
            if (!(entity is DeviceInfo))
                throw new ArgumentException(
                    "MemoryRamScanRepository expects DeviceInfo as entity",
                    nameof(entity)
                );

            var device = (DeviceInfo)entity;

            var miachineGuid = DeviceIdentityProvider.GetMachineGuid();
            var processor = new ProcessorCPUCollector().Collect();

            const string sql = @"
                INSERT INTO ProcessorCPUs (MachineGuid, Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeedMHz, LoadPercent, DeviceName)
                VALUES (@MachineGuid, @Name, @NumberOfCores, @NumberOfLogicalProcessors, @MaxClockSpeedMHz, @LoadPercent, @DeviceName);
            ";

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqliteParameter("@MachineGuid", miachineGuid.ToString()),
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

            var MachineGuid = DeviceIdentityProvider.GetMachineGuid();

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqliteParameter("@MachineGuid", MachineGuid.ToString())
            );
        }
    }
}
