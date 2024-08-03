namespace CFW.OData.Commands
{
    public class BoundFunctionCommand<TViewModel, TKey, TParameter>
    {
        public TKey Key { set; get; } = default!;

        public TParameter? Body { set; get; }
    }
}
