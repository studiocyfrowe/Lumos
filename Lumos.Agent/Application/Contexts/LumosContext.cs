using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Helpers;
using Lumos.Agent.Infrastructure.Factories;
using Microsoft.Data.Sqlite;

namespace Lumos.Agent.Application.Contexts
{
    public class LumosContext : LumosServiceContext
    {
        public QueryHelper Query { get; }

        private readonly string _connectionString;

        public LumosContext(IQueryHelperFactory factory)
        {
            Query = factory.Create();
            _connectionString = Query.ConnectionString;

            Initialize();
        }

        private void Initialize()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
             @"
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
            ";

            command.ExecuteNonQuery();
        }

        void LumosServiceContext.Initialize()
        {
            Initialize();
        }
    }
}
