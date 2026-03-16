using Lumos.Agent.Domain;
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

            var processor = new ProcessorCPUCollector().Collect();

            const string sql = @"
                INSERT INTO ProcessorCPUs (Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeedMHz, LoadPercent, DeviceName)
                VALUES (@Name, @NumberOfCores, @NumberOfLogicalProcessors, @MaxClockSpeedMHz, @LoadPercent, @DeviceName);
            ";

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqliteParameter("@Name", processor.Name),
                new SqliteParameter("@NumberOfCores", processor.NumberOfCores),
                new SqliteParameter("@NumberOfLogicalProcessors", processor.NumberOfLogicalProcessors),
                new SqliteParameter("@MaxClockSpeedMHz", processor.MaxClockSpeedMHz),
                new SqliteParameter("@LoadPercent", processor.LoadPercent),
                new SqliteParameter("@DeviceName", device.MachineName)
            );
        }

        public Task DeleteAsync(object value)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAsync(params object[] entities)
        {
            throw new NotImplementedException();
        }

        Task BaseRepositoryInterface<ProcessorCPU>.InsertAsync(object entity)
        {
            throw new System.NotImplementedException();
        }

        Task BaseRepositoryInterface<ProcessorCPU>.UpsertAsync(params object[] entities)
        {
            throw new System.NotImplementedException();
        }

        Task BaseRepositoryInterface<ProcessorCPU>.DeleteAsync(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
