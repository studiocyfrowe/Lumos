using Lumos.Agent.Application.Contexts;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Domain;
using Lumos.Agent.Domain.Models;
using Lumos.Agent.Infrastructure.Providers;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace Lumos.Agent.Application.Repositories
{
    public class ProcessRepository : BaseRepositoryInterface<ProcessModel>
    {
        private readonly LumosContext _context;
        private DeviceIdentityProvider identityProvider { get; set; }

        public ProcessRepository(LumosContext context, DeviceIdentityProvider identityProvider)
        {
            _context = context;
            this.identityProvider = identityProvider;
        }

        public async Task InsertAsync(object entity)
        {
            if (!(entity is ProcessModel processes))
                throw new ArgumentException(
                    "ProcessRepository expects ProcessModel as entity",
                    nameof(entity)
                );

            var machineGuid = this.identityProvider.GetMachineGuid();

            const string sql = @"
                CREATE TEMP TABLE TempProcesses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MachineGuid TEXT NOT NULL,
                    ProcessId INTEGER NOT NULL,
                    ProcessName TEXT NOT NULL,
                    MemoryUsageMB REAL NOT NULL,
                    CpuUsagePercent REAL NOT NULL,
                    StartTime TEXT NULL,
                    LastScan TEXT NOT NULL DEFAULT (datetime('now'))
                );

                INSERT INTO TempProcesses
                    (MachineGuid, ProcessId, ProcessName, MemoryUsageMB, CpuUsagePercent, StartTime, LastScan)
                VALUES
                    (@MachineGuid, @ProcessId, @ProcessName, @MemoryUsageMB, @CpuUsagePercent, @StartTime, @LastScan);

                INSERT INTO Processes
                    (MachineGuid, ProcessId, ProcessName, MemoryUsageMB, CpuUsagePercent, StartTime, LastScan)
                SELECT
                    MachineGuid,
                    ProcessId,
                    ProcessName,
                    MemoryUsageMB,
                    CpuUsagePercent,
                    StartTime,
                    LastScan
                FROM TempProcesses
                ORDER BY MemoryUsageMB DESC
                LIMIT 20;

                DROP TABLE TempProcesses;
            ";

            foreach (var process in processes.Processes)
            {
                await _context.Query.StoreOrUpdateDatabase<int>(
                    sql,
                    new SqliteParameter("@MachineGuid", process.MachineGuid),
                    new SqliteParameter("@ProcessId", process.ProcessId),
                    new SqliteParameter("@ProcessName", process.ProcessName),
                    new SqliteParameter("@MemoryUsageMB", process.MemoryUsageMB),
                    new SqliteParameter("@CpuUsagePercent", process.CpuUsagePercent),
                    new SqliteParameter("@StartTime", process.StartTime),
                    new SqliteParameter("@LastScan", DateTime.Now)
                );
            }
        }

        public async Task DeleteAsync(object value)
        {
            if (!(value is ProcessModel processes))
                throw new ArgumentException(
                    "ProcessRepository expects ProcessModel as entity",
                    nameof(value)
                );

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
                    FROM Processes
                    WHERE MachineGuid = @MachineGuid
                )
                DELETE FROM Processes
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
