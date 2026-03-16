using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Common.Infrastructure.Interfaces
{
    public interface BaseRepositoryInterface<T> where T : class
    {
        Task InsertAsync(object entity);
        Task UpsertAsync(params object[] entities);
        Task DeleteAsync(object value);
    }
}
