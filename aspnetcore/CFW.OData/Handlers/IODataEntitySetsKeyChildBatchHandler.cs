using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public interface IODataEntitySetsKeyChildBatchHandler<TViewModel, TKey, TChildViewModel, TChildKey>
        where TViewModel : class
        where TChildViewModel : class
    {
        Task<DomainResult> PatchModel(PatchEntitySetKeyChildSetCommand<TViewModel, TKey, TChildViewModel, TChildKey> command, CancellationToken cancellationToken = default);
    }
}
