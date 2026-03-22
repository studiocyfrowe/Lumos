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
);