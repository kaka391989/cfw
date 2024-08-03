using CFW.Core.Models;
using CFW.EFCore;
using Microsoft.EntityFrameworkCore;
using CFW.OData.Handlers;

namespace CFW.OData.Handlers
{
    public abstract class BaseQueryODataHandler<TModel, TKey> : IODataEntitySetsQueryHandler<TModel, TKey>
        where TModel : class, IEntity<TKey>
    {
        protected readonly AppDbContext _db;

        protected BaseQueryODataHandler(AppDbContext db)
        {
            _db = db;
        }

        public abstract IQueryable<TModel> Query { get; }

        public Task<TModel?> GetModel(TKey key, CancellationToken cancellationToken) =>
            Query.FirstOrDefaultAsync(x => x.Id!.Equals(key), cancellationToken);

        public Task<IQueryable<TModel>> GetQuery(CancellationToken cancellationToken) => Task.FromResult(Query);
    }
}
