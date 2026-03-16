using Lumos.Agent.Helpers.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lumos.Agent.Helpers
{
    public class QueryHelper : QueryHelperInterface
    {
        public string ConnectionString { get; }

        public QueryHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<T> RetrieveResults<T>(
            string query,
            Func<SqliteDataReader, T> mapFunction,
            params SqliteParameter[] parameters)
        {
            var results = new List<T>();

            using (var conn = new SqliteConnection(ConnectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(mapFunction(reader));
                    }
                }
            }

            return results;
        }

        public T RetrieveSingleResult<T>(
            string query,
            params SqliteParameter[] parameters)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return default;

                return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        public async Task<T> StoreOrUpdateDatabase<T>(
            string query,
            params SqliteParameter[] parameters)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();

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

        public bool RemoveFromDatabase(
            string query,
            params SqliteParameter[] parameters)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            using (var cmd = new SqliteCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool RemoveFromDatabase<T>(string query, params SqliteParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}