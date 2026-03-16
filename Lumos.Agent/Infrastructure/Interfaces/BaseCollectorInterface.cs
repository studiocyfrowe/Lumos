namespace Lumos.Agent.Infrastructure.Interfaces
{
    public interface BaseCollectorInterface<T> where T : class
    {
        T Collect();
    }
}
