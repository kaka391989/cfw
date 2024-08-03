using Microsoft.AspNetCore.OData.Deltas;

namespace CFW.OData.Commands
{
    public class PatchCommand<TViewModel, TKey>
        where TViewModel : class
    {
        public TKey Key { get; set; } = default!;

        public Delta<TViewModel> Delta { get; set; } = default!;
    }

    public class PatchEntitySetKeyChildSetCommand<TViewModel, TKey, TChildViewModel, TChildKey>
        where TViewModel : class
        where TChildViewModel : class
    {
        public TKey Key { get; set; } = default!;

        public TChildKey ChildKey { get; set; } = default!;

        public Delta<TChildViewModel> Delta { get; set; } = default!;
    }

    public class PostEntitySetKeyChildSetCommand<TViewModel, TKey, TChildViewModel, TChildKey>
        where TViewModel : class
        where TChildViewModel : class
    {
        public TKey Key { get; set; } = default!;

        public TChildViewModel Body { get; set; } = default!;
    }
}
