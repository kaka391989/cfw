namespace CFW.OData.Commands
{
    public class CreateCommand<TViewModel, Tkey>
    {
        public TViewModel Model { get; set; } = default!;
    }
}
