using Lumos.Agent.Helpers;
using System;

namespace Lumos.Agent.Infrastructure.Factories
{
    public interface IQueryHelperFactory
    {
        QueryHelper Create();
    }

    public class QueryHelperFactory : IQueryHelperFactory
    {
        private readonly string _connectionString;

        public QueryHelperFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.");

            _connectionString = connectionString;
        }

        public QueryHelper Create()
        {
            return new QueryHelper(_connectionString);
        }
    }
}