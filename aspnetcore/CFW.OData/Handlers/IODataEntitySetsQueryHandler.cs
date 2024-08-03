using Microsoft.AspNetCore.OData.Query;

namespace CFW.OData.Handlers
{
    public interface IODataEntitySetsQueryHandler<TViewModel, TKey>
    {
        Task<IQueryable<TViewModel>> GetQuery(CancellationToken cancellationToken);

        Task<TViewModel?> GetModel(TKey key, CancellationToken cancellationToken);
    }

    public interface IODataEntitySetsGetPropertyByKeyHandler<TViewModel, TKey>
    {
        Task<object?> GetProperty(TKey key, string propertyName, CancellationToken cancellationToken);
    }

    public interface IODataEntitySetsCustomQueryHandler<TViewModel, TKey>
    {
        Task<IQueryable<TViewModel>> GetQuery(ODataQueryOptions<TViewModel> options, CancellationToken cancellationToken);
    }
}
