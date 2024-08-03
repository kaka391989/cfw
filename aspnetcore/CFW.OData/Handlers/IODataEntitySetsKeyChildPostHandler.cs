using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public interface IODataEntitySetsKeyChildPostHandler<TViewModel, TKey, TChildViewModel, TChildKey>
        where TViewModel : class
        where TChildViewModel : class
    {
        Task<DomainResult> CreateChildModel(PostEntitySetKeyChildSetCommand<TViewModel, TKey, TChildViewModel, TChildKey> command, CancellationToken cancellationToken = default);
    }
}
