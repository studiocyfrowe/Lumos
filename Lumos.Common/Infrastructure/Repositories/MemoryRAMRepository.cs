using Lumos.Agent.Models;
using Lumos.Common.Infrastructure.Interfaces;
using Microsoft.Data.Sqlite;

namespace Lumos.Agent.Repositories
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
                    (@MachineGuid, @StationName, @TotalGB, @UsedGB, @FreeGB, @UsedPercent, @LastScan)
            ";

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqliteParameter("@MachineGuid", machineGuid.ToString()),
                new SqliteParameter("@StationName", device.MachineName),
                new SqliteParameter("@TotalGB", memory.TotalGB),
                new SqliteParameter("@UsedGB", memory.UsedGB),
                new SqliteParameter("@FreeGB", memory.FreeGB),
                new SqliteParameter("@UsedPercent", memory.UsedPercent),
                new SqliteParameter("@LastScan", DateTime.UtcNow.ToString("o"))
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
                new SqliteParameter("@MachineGuid", MachineGuid.ToString())
            );
        }

        public async Task UpsertAsync(params object[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
