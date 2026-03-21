using Lumos.Agent.Application.Contexts;
using Lumos.Agent.Application.Interfaces;
using Lumos.Agent.Domain;
using Lumos.Agent.Infrastructure.Interfaces;
using Lumos.Agent.Infrastructure.Providers;
using Lumos.Agent.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace Lumos.Agent.Application.Repositories
{
    public class DeviceInfoRepository : BaseRepositoryInterface<DeviceInfo>
    {
        private readonly LumosContext _context;
        private DeviceIdentityProvider identityProvider { get; set; }
        private readonly BaseCollectorInterface<DeviceInfo> _deviceInfoCollector;

        public DeviceInfoRepository(
            LumosContext context, 
            DeviceIdentityProvider identityProvider, 
            BaseCollectorInterface<DeviceInfo> deviceInfoCollector)
        {
            _context = context;
            this.identityProvider = identityProvider;
            _deviceInfoCollector = deviceInfoCollector; 
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

                var MachineGuid = this.identityProvider.GetMachineGuid();

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
