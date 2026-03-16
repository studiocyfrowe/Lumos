using Lumos.Agent.Helpers;
using Lumos.Agent.Infrastructure.Factories;
using Lumos.Common.Infrastructure.Interfaces;
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
        ";

        command.ExecuteNonQuery();
    }
}