using Lumos.Agent.Application.Helpers;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Helpers;
using Lumos.Agent.Infrastructure.Factories;
using Microsoft.Data.Sqlite;

namespace Lumos.Agent.Application.Contexts
{
    public class LumosContext : LumosServiceContext
    {
        public QueryHelper Query { get; }
        private SqlLoader SqlLoader;

        private readonly string _connectionString;

        public LumosContext(IQueryHelperFactory factory, SqlLoader sqlLoader)
        {
            Query = factory.Create();
            _connectionString = Query.ConnectionString;
            this.SqlLoader = sqlLoader;

            Initialize();
        }

        private void Initialize()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var sql = @"
               CREATE TABLE IF NOT EXISTS MemoryRamScans (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MachineGuid TEXT NOT NULL,
    StationName TEXT NOT NULL,
    TotalGB REAL NOT NULL,
    UsedGB REAL NOT NULL,
    FreeGB REAL NOT NULL,
    UsedPercent REAL NOT NULL,
    LastScan TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS ProcessorCPUs
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MachineGuid TEXT NOT NULL,
    Name TEXT NOT NULL,
    NumberOfCores INTEGER NOT NULL,
    NumberOfLogicalProcessors INTEGER NOT NULL,
    MaxClockSpeedMHz INTEGER NOT NULL,
    LoadPercent INTEGER NOT NULL,
    DeviceName TEXT NULL,
    LastScan TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Processes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MachineGuid TEXT NOT NULL,
    ProcessId INTEGER NOT NULL,
    ProcessName TEXT NOT NULL,
    MemoryUsageMB REAL NOT NULL,
    CpuUsagePercent REAL NOT NULL,
    StartTime TEXT NULL,
    LastScan TEXT NOT NULL DEFAULT (datetime('now'))
);";

            var command = connection.CreateCommand();
            command.CommandText = sql;

            command.ExecuteNonQuery();
        }

        void LumosServiceContext.Initialize()
        {
            Initialize();
        }
    }
}
