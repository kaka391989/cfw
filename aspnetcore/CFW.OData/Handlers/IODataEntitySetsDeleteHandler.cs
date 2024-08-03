using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public interface IODataEntitySetsDeleteHandler<TViewModel, TKey>
        where TViewModel : class
    {
        Task<DomainResult<TKey>> DeleteModel(DeleteCommand<TViewModel, TKey> command
            , CancellationToken cancellationToken = default);
    }
}
