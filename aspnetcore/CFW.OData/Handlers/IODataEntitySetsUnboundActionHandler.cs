using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public interface IODataEntitySetsUnboundActionHandler<TViewModel, TKey, TBody>
        where TViewModel : class
    {
        Task<DomainResult> Execute(UnboundActionCommand<TViewModel, TKey, TBody> command, CancellationToken cancellationToken);
    }

    public interface IODataEntitySetsUnboundActionHandler<TViewModel, TKey, TBody, TReturn>
        where TViewModel : class
    {
        Task<DomainResult<TReturn>> Execute(UnboundActionCommand<TViewModel, TKey, TBody, TReturn> command, CancellationToken cancellationToken);
    }
}
