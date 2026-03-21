using Lumos.Agent.Helpers;
using Lumos.Agent.Infrastructure.Factories;
using Lumos.Agent.Infrastructure.Interfaces;
using Microsoft.Data.Sqlite;

public class ApplicationDbContext : Hades2MonitorContextInterface
{
    public QueryHelper Query { get; }

    private readonly string _connectionString;

    public ApplicationDbContext(IQueryHelperFactory factory)
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
                Id INT IDENTITY(1,1) PRIMARY KEY,
                MachineGuid UNIQUEIDENTIFIER NOT NULL,
                Name NVARCHAR(255) NOT NULL,
                NumberOfCores INT NOT NULL,
                NumberOfLogicalProcessors INT NOT NULL,
                MaxClockSpeedMHz INT NOT NULL,
                LoadPercent INT NOT NULL,
                DeviceName NVARCHAR(255) NULL,
                LastScan DATETIME2 NOT NULL DEFAULT SYSDATETIME()
            );
        ";

        command.ExecuteNonQuery();
    }
}