using CFW.Core.DomainResults;
using CFW.Core.Models;
using CFW.Core.Utils;
using CFW.EFCore;
using CFW.OData.Commands;

namespace CFW.OData.Handlers
{
    public abstract class BaseCreateODataHandler<TModel, TDbModel, TKey> : IODataEntitySetsCreateHandler<TModel, TKey>
        where TModel : class, IEntity<TKey>
        where TDbModel : class, IEntity<TKey>
    {
        protected readonly AppDbContext _db;

        protected BaseCreateODataHandler(AppDbContext db)
        {
            _db = db;
        }

        public virtual async Task<DomainResult<TKey>> CreateModel(CreateCommand<TModel, TKey> command, CancellationToken cancellationToken = default)
        {
            var dbModel = command.Model.Convert<TDbModel>();
            _db.Set<TDbModel>().Add(dbModel);
            await _db.SaveChangesAsync();

            return DomainResult.Success(dbModel.Id!);
        }

        public virtual async Task<DomainResult<TChildKey>> CreateChildModel<TChildModel, TChildKey>(
            PostEntitySetKeyChildSetCommand<TModel, TKey, TChildModel, TChildKey> command
            , CancellationToken cancellationToken = default)
            where TChildModel : class
        {
            throw new Exception();
        }
    }
}
