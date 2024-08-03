
using CFW.Core.DomainResults;

namespace CFW.OData.Handlers
{
    public interface IODataSingletonActionHandler<TViewModel>
        where TViewModel : class
    {
        Task<DomainResult> Execute(CancellationToken cancellationToken);
    }
}
