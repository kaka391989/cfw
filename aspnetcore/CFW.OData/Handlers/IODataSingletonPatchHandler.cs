using CFW.Core.DomainResults;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public interface IODataSingletonPatchHandler<TViewModel>
        where TViewModel : class
    {
        Task<DomainResult> PatchSingleModel(BatchSingleCommand<TViewModel> command, CancellationToken cancellationToken = default);
    }
}
