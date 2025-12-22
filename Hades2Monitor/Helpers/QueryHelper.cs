using Hades2Monitor.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Hades2Monitor.Helpers
{
    public class QueryHelper : QueryHelperInterface
    {
        private string _connectionString { get; set; }

        public QueryHelper(string connectionString)
        {
            this._connectionString = connectionString;
        }


        public List<T> RetrieveResults<T>(string query,
                                Func<SqlDataReader, T> mapFunction,
                                params SqlParameter[] parameters)
        {
            List<T> results = new List<T>();

            try
            {
                using (SqlConnection conn = new SqlConnection(this._connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = mapFunction(reader);
                            results.Add(item);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Error: {ex.Message}", ex);
            }

            return results;
        }

        public T RetrieveSingleResult<T>(string query,
                                    params SqlParameter[] parameters)
        {
            T singleResult = default;

            try
            {
                using (SqlConnection conn = new SqlConnection(this._connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.OpenAsync();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        singleResult = reader.Read() ? (T)reader[0] : default;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Error: {ex.Message}", ex);
            }

            return singleResult;
        }

        public async Task<T> StoreOrUpdateDatabase<T>(string query,
                                              params SqlParameter[] parameters)
        {
            try
            {
                using (var sqliteconn = new SqlConnection(this._connectionString))
                using (var cmd = new SqlCommand(query, sqliteconn))
                {
                    if (parameters?.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    await sqliteconn.OpenAsync();

                    if (typeof(T) == typeof(int))
                    {
                        int rows = await cmd.ExecuteNonQueryAsync();
                        return (T)(object)rows;
                    }

                    object result = await cmd.ExecuteScalarAsync();

                    if (result == null || result == DBNull.Value)
                        return default;

                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"TEST UPDATE: {ex.Message}");
                throw;
            }
        }

        public bool RemoveFromDatabase<T>(string query, params SqlParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
