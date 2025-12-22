using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Hades2Monitor.Helpers.Interfaces
{
    public interface QueryHelperInterface
    {
        List<T> RetrieveResults<T>(string query,
                                Func<SqlDataReader, T> mapFunction,
                                params SqlParameter[] parameters);

        T RetrieveSingleResult<T>(string query,
                                params SqlParameter[] parameters);

        Task<T> StoreOrUpdateDatabase<T>(string query,
                                    params SqlParameter[] parameters);

        bool RemoveFromDatabase<T>(string query,
                                    params SqlParameter[] parameters);
    }
}
