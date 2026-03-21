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
    public class MemoryRAMRepository : BaseRepositoryInterface<MemoryRAM>
    {
        private readonly LumosContext _context;
        private DeviceIdentityProvider identityProvider { get; set; }
        private readonly BaseCollectorInterface<MemoryRAM> _memoryRAMCollector;

        public MemoryRAMRepository(LumosContext context, 
            DeviceIdentityProvider identityProvider,
            BaseCollectorInterface<MemoryRAM> memoryRAMCollector)
        {
            _context = context;
            this.identityProvider = identityProvider;
            _memoryRAMCollector = memoryRAMCollector;
        }

        public async Task InsertAsync(object entity)
        {
            if (!(entity is DeviceInfo))
                throw new ArgumentException(
                    "MemoryRamScanRepository expects DeviceInfo as entity",
                    nameof(entity)
                );

            var device = (DeviceInfo)entity;

            var memory = _memoryRAMCollector.Collect();
            var machineGuid = this.identityProvider.GetMachineGuid();

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
                DELETE FROM MemoryRamScans
                WHERE Id IN
                (
                    SELECT Id
                    FROM Ranked
                    WHERE rn > 60
                );
            ";

            var machineGuid = this.identityProvider.GetMachineGuid();

            await _context.Query.StoreOrUpdateDatabase<int>(
                sql,
                new SqliteParameter("@MachineGuid", machineGuid.ToString())
            );
        }
    }
}
