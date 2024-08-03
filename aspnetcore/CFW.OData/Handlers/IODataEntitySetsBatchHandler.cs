using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{

    public interface IODataEntitySetsBatchHandler<TViewModel, TKey>
        where TViewModel : class
    {
        Task<DomainResult> PatchModel(PatchCommand<TViewModel, TKey> command, CancellationToken cancellationToken = default);
    }
}
