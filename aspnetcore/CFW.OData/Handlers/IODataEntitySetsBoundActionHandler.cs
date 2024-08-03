using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public interface IODataEntitySetsBoundActionHandler<TViewModel, TKey, TBody, TReturn>
        where TViewModel : class
    {
        Task<DomainResult<TReturn>> Execute(ActionCommand<TViewModel, TKey, TBody, TReturn> command, CancellationToken cancellationToken);
    }

    public interface IODataEntitySetsBoundActionHandler<TViewModel, TKey, TBody>
        where TViewModel : class
    {
        Task<DomainResult> Execute(ActionCommand<TViewModel, TKey, TBody> command, CancellationToken cancellationToken);
    }
}
