using Hades2Monitor.Helpers;
using Microsoft.Extensions.Configuration;
using System;

namespace Hades2Monitor.Factories
{
    public interface IQueryHelperFactory
    {
        QueryHelper Create(string connectionStringName);
    }

    public class QueryHelperFactory : IQueryHelperFactory
    {
        private readonly IConfiguration _configuration;

        public QueryHelperFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public QueryHelper Create(string connectionStringName)
        {
            string conn = _configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException(
                    $"Connection string '{connectionStringName}' nie została zdefiniowana w konfiguracji.");

            return new QueryHelper(conn);
        }

    }
}
