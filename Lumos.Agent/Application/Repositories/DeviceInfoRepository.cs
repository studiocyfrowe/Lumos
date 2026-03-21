using Lumos.Agent.Domain;
using Lumos.Agent.Domain.Providers;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace Lumos.Agent.Repositories
{
    public class DeviceInfoRepository : BaseRepositoryInterface<DeviceInfo>
    {
        private readonly ApplicationDbContext _context;
        public DeviceInfoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task DeleteAsync(object value)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(object entity)
        {
            throw new NotImplementedException();
        }

        public async Task UpsertAsync(params object[] entities)
        {
            foreach (var entity in entities)
            {
                if (!(entity is DeviceInfo device))
                    throw new ArgumentException(
                        "DeviceInfoRepository expects DeviceInfo as entity",
                        nameof(entity)
                    );

                if (!(entity is UserSessionInfo user))
                    throw new ArgumentException(
                        "DeviceInfoRepository expects UserSessionInfo as entity",
                        nameof(entity)
                    );

                string sql = @"
                    MERGE Workstations AS target
                    USING (SELECT @MachineGuid AS MachineGuid) AS source
                    ON target.MachineGuid = source.MachineGuid
                    WHEN MATCHED THEN
                        UPDATE SET
                            MachineName = @MachineName,
                            OSVersion = @OSVersion,
                            LoggedUser = @UserName,
                            LoggedUserDomain = @Domain,
                            LastSeen = SYSDATETIME()
                    WHEN NOT MATCHED THEN
                        INSERT (
                            Id, MachineGuid, MachineName, OSVersion,
                            LoggedUser, LoggedUserDomain, LastSeen
                        )
                        VALUES (
                            NEWID(), @MachineGuid, @MachineName, @OSVersion,
                            @UserName, @Domain, SYSDATETIME()
                        );";

                var MachineGuid = DeviceIdentityProvider.GetMachineGuid();

                await _context.Query.StoreOrUpdateDatabase<int>(
                    sql,
                    new SqliteParameter("@MachineGuid", "Test"),
                    new SqliteParameter("@MachineName", device.MachineName),
                    new SqliteParameter("@OSVersion", device.OSVersion),
                    new SqliteParameter("@UserName",
                        user?.UserName ?? (object)DBNull.Value),
                    new SqliteParameter("@Domain",
                        user?.Domain ?? (object)DBNull.Value)
                );
            }
        }
    }
}
