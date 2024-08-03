using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public interface IODataEntitySetsCreateHandler<TViewModel, TKey>
        where TViewModel : class
    {
        Task<DomainResult<TKey>> CreateModel(CreateCommand<TViewModel, TKey> command, CancellationToken cancellationToken = default);
    }
}
