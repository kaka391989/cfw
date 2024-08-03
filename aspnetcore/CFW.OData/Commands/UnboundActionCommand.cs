namespace CFW.OData.Commands
{
    public class UnboundActionCommand<TViewModel, TKey, TBody>
    {
        public TBody Body { set; get; } = default!;
    }

    public class UnboundActionCommand<TViewModel, TKey, TBody, TReturn>
    {
        public TBody Body { set; get; } = default!;
    }
}
