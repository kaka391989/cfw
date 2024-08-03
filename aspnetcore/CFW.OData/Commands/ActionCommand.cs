namespace CFW.OData.Commands
{
    public class ActionCommand<TViewModel, TKey, TBody, TReturn>
    {
        public TKey Key { set; get; } = default!;

        public TBody? Body { set; get; }
    }

    public class ActionCommand<TViewModel, TKey, TBody>
    {
        public TKey Key { set; get; } = default!;

        public TBody? Body { set; get; }
    }
}
