using Microsoft.AspNetCore.OData.Deltas;

namespace CFW.OData.Commands
{

    public class BatchSingleCommand<TViewModel>
        where TViewModel : class
    {
        public Delta<TViewModel> Delta { get; set; } = default!;
    }
}
