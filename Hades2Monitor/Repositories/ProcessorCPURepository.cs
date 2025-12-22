using Hades2Monitor.Collectors;
using Hades2Monitor.Models;
using Hades2Monitor.Repositories.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Hades2Monitor.Repositories
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
                new SqlParameter("@Name", SqlDbType.NVarChar)
                {
                    Value = processor.Name
                },
                new SqlParameter("@NumberOfCores", SqlDbType.NVarChar)
                {
                    Value = processor.NumberOfCores
                },
                new SqlParameter("@NumberOfLogicalProcessors", SqlDbType.NVarChar)
                {
                    Value = processor.NumberOfLogicalProcessors
                },
                new SqlParameter("@MaxClockSpeedMHz", SqlDbType.NVarChar)
                {
                    Value = processor.MaxClockSpeedMHz
                },
                new SqlParameter("@LoadPercent", SqlDbType.NVarChar)
                {
                    Value = processor.LoadPercent
                },
                new SqlParameter("@DeviceName", SqlDbType.NVarChar)
                {
                    Value = device.MachineName
                }
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
    }
}
