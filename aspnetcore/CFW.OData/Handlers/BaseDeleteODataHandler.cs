using CFW.Core.DomainResults;
using CFW.Core.Models;
using CFW.EFCore;
using CFW.OData.Commands;
using CFW.OData.Handlers;

namespace CFW.OData.Handlers
{
    public abstract class BaseDeleteODataHandler<TModel, TDbModel, TKey> : IODataEntitySetsDeleteHandler<TModel, TKey>
        where TModel : class, IEntity<TKey>
        where TDbModel : class, IEntity<TKey>
    {
        protected readonly AppDbContext _db;

        protected BaseDeleteODataHandler(AppDbContext db)
        {
            _db = db;
        }

        public virtual async Task<DomainResult<TKey>> DeleteModel(DeleteCommand<TModel, TKey> command, CancellationToken cancellationToken = default)
        {
            var dbModel = await _db.Set<TDbModel>().FindAsync(command.Key, cancellationToken);
            if (dbModel is null)
            {
                return DomainResult.NotFound<TKey>();
            }

            _db.Set<TDbModel>().Remove(dbModel);
            await _db.SaveChangesAsync(cancellationToken);

            return DomainResult.Success(command.Key);
        }
    }
}
