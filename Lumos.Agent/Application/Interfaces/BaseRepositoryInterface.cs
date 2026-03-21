using System.Threading.Tasks;

namespace Lumos.Agent.Application.Interfaces
{
    public interface BaseRepositoryInterface<T> where T : class
    {
        Task InsertAsync(object entity);
        Task DeleteAsync(object value);
    }
}
