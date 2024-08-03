namespace CFW.OData.Commands
{
    public class DeleteCommand<TViewModel, TKey>
    {
        public TKey Key { get; set; } = default!;
    }
}
