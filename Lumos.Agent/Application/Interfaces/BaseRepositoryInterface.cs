using System.Threading.Tasks;

namespace Lumos.Agent.Infrastructure.Interfaces
{
    public interface BaseRepositoryInterface<T> where T : class
    {
        Task InsertAsync(object entity);
        Task DeleteAsync(object value);
    }
}
