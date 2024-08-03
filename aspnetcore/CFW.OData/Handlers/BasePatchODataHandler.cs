using CFW.Core.DomainResults;
using CFW.Core.Models;
using CFW.Core.Utils;
using CFW.EFCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CFW.OData.Commands;
using CFW.OData.Handlers;

namespace CFW.OData.Handlers
{
    public abstract class BasePatchODataHandler<TModel, TDbModel, TKey> :
        IODataEntitySetsBatchHandler<TModel, TKey>
        where TModel : class, IEntity<TKey>
        where TDbModel : class, IEntity<TKey>
    {
        protected readonly AppDbContext _db;

        protected BasePatchODataHandler(AppDbContext db)
        {
            _db = db;
        }

        protected virtual async Task<DomainResult> PatchChildModel<TPatchModel, TDbPatchModel, TPatchKey>(
            PatchCommand<TPatchModel, TPatchKey> command,
            CancellationToken cancellationToken = default
        )
            where TPatchModel : class, IEntity<TPatchKey>
            where TDbPatchModel : class, IEntity<TPatchKey>
        {
            var changed = command.Delta.GetChangedPropertyNames();

            if (!changed.Any())
                return DomainResult.Success();

            var changedProperties = new Dictionary<string, object?>();
            foreach (var property in changed)
            {
                if (command.Delta.TryGetPropertyValue(property, out var value))
                    changedProperties[property] = value;
            }

            var dbModel = await _db.Set<TDbPatchModel>().FirstOrDefaultAsync(x => x.Id!.Equals(command.Key), cancellationToken);

            var changedPropertiesJson = changedProperties.ToJson();
            JsonConvert.PopulateObject(changedPropertiesJson, dbModel!);

            await _db.SaveChangesAsync(cancellationToken);

            return DomainResult.Success();
        }

        public virtual async Task<DomainResult> PatchModel(PatchCommand<TModel, TKey> command, CancellationToken cancellationToken = default)
        {
            var changed = command.Delta.GetChangedPropertyNames();

            if (!changed.Any())
                return DomainResult.Success();

            var changedProperties = new Dictionary<string, object?>();
            foreach (var property in changed)
            {
                if (command.Delta.TryGetPropertyValue(property, out var value))
                    changedProperties[property] = value;
            }

            var dbModel = await _db.Set<TDbModel>().FirstOrDefaultAsync(x => x.Id!.Equals(command.Key), cancellationToken);

            var changedPropertiesJson = changedProperties.ToJson();
            JsonConvert.PopulateObject(changedPropertiesJson, dbModel!);

            await _db.SaveChangesAsync(cancellationToken);

            return DomainResult.Success();
        }
    }
}
