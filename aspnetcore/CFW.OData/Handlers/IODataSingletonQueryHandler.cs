namespace CFW.OData.Handlers
{
    public interface IODataSingletonQueryHandler<TViewModel>
    {
        public Task<TViewModel?> GetModel(CancellationToken cancellationToken);
    }
}
