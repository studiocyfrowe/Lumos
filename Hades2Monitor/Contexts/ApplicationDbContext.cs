using Hades2Monitor.Contexts.Interfaces;
using Hades2Monitor.Factories;
using Hades2Monitor.Helpers;

public class ApplicationDbContext : Hades2MonitorContextInterface
{
    public QueryHelper Query { get; }

    public ApplicationDbContext(IQueryHelperFactory factory)
    {
        Query = factory.Create("DefaultDatabase");
    }
}
