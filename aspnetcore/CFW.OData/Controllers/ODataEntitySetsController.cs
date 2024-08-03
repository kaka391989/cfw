using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using CFW.OData.Conventions;
using CFW.OData.Extensions;

namespace CFW.OData.Controllers
{
    //Handle Get, GetList, Post, Patch, Delete of OData entity sets.
    [ODataEntitySetsConvention]
    internal class ODataEntitySetsController<TViewModel, TKey> : ODataController
        where TViewModel : class
    {
        [HttpGet]
        [EnableQuery]
        public Task<IActionResult> GetEntitySets(ODataQueryOptions<TViewModel> options, CancellationToken cancellationToken)
            => this.HandleGetEntitySets(options, cancellationToken);

        [HttpGet]
        [EnableQuery]
        public Task<IActionResult> GetByKey([FromODataUri] TKey key, CancellationToken cancellationToken)
            => this.HandleGetEntitySetByKey(key, cancellationToken);

        [HttpGet]
        [EnableQuery]
        public Task<IActionResult> GetPropertyValueByKey(
        [FromODataUri] TKey key,
        [FromRoute] string propertyName,
        ODataQueryOptions<TViewModel> options,
        CancellationToken cancellationToken)
            => this.HandleGetPropertyValueByKey(key, propertyName, options, cancellationToken);

        [HttpPost]
        [EnableQuery]
        public Task<IActionResult> Create([FromBody] TViewModel entity, CancellationToken cancellationToken)
            => this.CreateEntitySet(entity, cancellationToken);

        [HttpPatch]
        [EnableQuery]
        public Task<IActionResult> Patch([FromODataUri] TKey key, [FromBody] Delta<TViewModel> entity, CancellationToken cancellationToken)
            => this.PatchEntitySet(key, entity, cancellationToken);

        [HttpDelete]
        [EnableQuery]
        public Task<IActionResult> Delete([FromODataUri] TKey key, CancellationToken cancellationToken)
            => this.DeleteEntitySet(key, cancellationToken);

    }
}
