using CFW.Core.DomainResults;
using CFW.Core.Models;
using CFW.Core.Utils;
using CFW.EFCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CFW.OData.Commands;
using CFW.OData.Handlers;

namespace CFW.OData.Handlers
{
    public abstract class BaseODataHandler<TModel, TDbModel, TKey> :
        IODataEntitySetsCreateHandler<TModel, TKey>
        where TModel : class, IEntity<TKey>
        where TDbModel : class, IEntity<TKey>
    {
        protected readonly AppDbContext _db;

        protected BaseODataHandler(AppDbContext db)
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
    }

    public abstract class BaseODataHandler<TModel, TDbModel, TKey, TPropertySet1, TPropertyDbSet1, TPropertySetKey>
        : BaseODataHandler<TModel, TDbModel, TKey>,
        IODataEntitySetsKeyChildPostHandler<TModel, TKey, TPropertySet1, TPropertySetKey>
        where TModel : class, IEntity<TKey>
        where TDbModel : class, IEntity<TKey>
        where TPropertySet1 : class, IEntity<TPropertySetKey>
        where TPropertyDbSet1 : class, IEntity<TPropertySetKey>
    {

        protected BaseODataHandler(AppDbContext db) : base(db)
        {
        }

        public async Task<DomainResult> CreateChildModel(
            PostEntitySetKeyChildSetCommand<TModel, TKey, TPropertySet1, TPropertySetKey> command
            , CancellationToken cancellationToken = default)
        {
            var prop = command.Body.Convert<TPropertyDbSet1>();
            var mainSetProp = typeof(TPropertyDbSet1).GetProperties().Single(x => x.PropertyType == typeof(TDbModel));

            var foreignKey = mainSetProp.GetCustomAttribute<ForeignKeyAttribute>()?.Name;
            if (foreignKey != null)
            {
                var foreignKeyProp = typeof(TPropertyDbSet1).GetProperty(foreignKey)!;
                foreignKeyProp.SetValue(prop, command.Key);
            }
            else
            {
                throw new InvalidOperationException("Foreign key not found");
            }

            _db.Set<TPropertyDbSet1>()
                .Add(prop);

            await _db.SaveChangesAsync(cancellationToken);

            return DomainResult.Success();
        }
    }
}
