using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lumos.Agent.Helpers.Interfaces
{
    public interface QueryHelperInterface
    {
        List<T> RetrieveResults<T>(string query,
                                Func<SqliteDataReader, T> mapFunction,
                                params SqliteParameter[] parameters);

        T RetrieveSingleResult<T>(string query,
                                params SqliteParameter[] parameters);

        Task<T> StoreOrUpdateDatabase<T>(string query,
                                    params SqliteParameter[] parameters);

        bool RemoveFromDatabase<T>(string query,
                                    params SqliteParameter[] parameters);
    }
}
