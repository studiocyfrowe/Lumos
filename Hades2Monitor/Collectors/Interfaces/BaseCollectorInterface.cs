using Hades2Monitor.Models;

namespace Hades2Monitor.Collectors.Interfaces
{
    public interface BaseCollectorInterface<T> where T : class
    {
        T Collect();
    }
}
