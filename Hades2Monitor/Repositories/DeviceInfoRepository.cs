using Hades2Monitor.Models;
using Hades2Monitor.Providers;
using Hades2Monitor.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Hades2Monitor.Repositories
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
                    new SqlParameter("@MachineGuid", SqlDbType.NVarChar, 4000)
                    {
                        Value = "Test"
                    },
                    new SqlParameter("@MachineName", SqlDbType.NVarChar, 15)
                    {
                        Value = device.MachineName
                    },
                    new SqlParameter("@OSVersion", SqlDbType.NVarChar, 50)
                    {
                        Value = device.OSVersion
                    },
                    new SqlParameter("@UserName", SqlDbType.NVarChar, 256)
                    {
                        Value = user != null && user.UserName != null
                        ? (object)user.UserName
                        : DBNull.Value
                    },
                    new SqlParameter("@Domain", SqlDbType.NVarChar, 256)
                    {
                        Value = user != null && user.Domain != null
                            ? (object)user.Domain
                            : DBNull.Value
                    }
                );
            }
        }
    }
}
