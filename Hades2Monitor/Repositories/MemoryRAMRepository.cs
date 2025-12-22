using Hades2Monitor.Collectors;
using Hades2Monitor.Models;
using Hades2Monitor.Providers;
using Hades2Monitor.Repositories.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Hades2Monitor.Repositories
{
    public class MemoryRAMRepository : BaseRepositoryInterface<MemoryRAM>
    {
        private readonly ApplicationDbContext _context;
        public MemoryRAMRepository(ApplicationDbContext context)
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

            var memory = new MemoryRAMCollector().Collect();
            var machineGuid = DeviceIdentityProvider.GetMachineGuid();

            const string sql = @"
                INSERT INTO MemoryRamScans
                    (MachineGuid, StationName, TotalGB, UsedGB, FreeGB, UsedPercent, LastScan)
                VALUES
                    (@MachineGuid, @StationName, @TotalGB, @UsedGB, @FreeGB, @UsedPercent, SYSDATETIME())
            ";

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqlParameter("@MachineGuid", SqlDbType.UniqueIdentifier)
                {
                    Value = machineGuid
                },
                new SqlParameter("@StationName", SqlDbType.NVarChar, 50)
                {
                    Value = device.MachineName
                },
                new SqlParameter("@TotalGB", SqlDbType.Float)
                {
                    Value = memory.TotalGB
                },
                new SqlParameter("@UsedGB", SqlDbType.Float)
                {
                    Value = memory.UsedGB
                },
                new SqlParameter("@FreeGB", SqlDbType.Float)
                {
                    Value = memory.FreeGB
                },
                new SqlParameter("@UsedPercent", SqlDbType.Float)
                {
                    Value = memory.UsedPercent
                }
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
                    FROM MemoryRamScans
                    WHERE MachineGuid = @MachineGuid
                )
                DELETE FROM Ranked
                WHERE rn > 60;
            ";

            var MachineGuid = DeviceIdentityProvider.GetMachineGuid();

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqlParameter("@MachineGuid", SqlDbType.NVarChar, 4000)
                {
                    Value = MachineGuid.ToString()
                }
            );
        }

        public async Task UpsertAsync(params object[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
