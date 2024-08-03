using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using CFW.OData.Commands;
using CFW.OData.Conventions;
using CFW.OData.Handlers;

namespace CFW.OData.Controllers;

[ODataEntitySetKeyChildSetConvention]
internal class ODataEntitySetKeyChildSetsController<TViewModel, TKey, TChildViewModel, TChildKey> : ODataController
    where TChildViewModel : class
    where TViewModel : class
{
    [HttpPost]
    public async Task<IActionResult> PostChild(
        [FromODataUri] TKey key,
        [FromBody] TChildViewModel childModel,
        [FromServices] IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var handler = serviceProvider.GetService<IODataEntitySetsKeyChildPostHandler<TViewModel, TKey, TChildViewModel, TChildKey>>();
        if (handler == null)
        {
            return NotFound();
        }

        var resut = await handler.CreateChildModel(new PostEntitySetKeyChildSetCommand<TViewModel, TKey, TChildViewModel, TChildKey>
        {
            Key = key,
            Body = childModel
        }, cancellationToken);

        return resut.ToActionResult();
    }

    [HttpPatch]
    [EnableQuery]
    public async Task<IActionResult> PatchChild(
        [FromODataUri] TKey key,
        [FromODataUri] TChildKey childKey,
        [FromBody] Delta<TChildViewModel> delta,
        [FromServices] IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var handler = serviceProvider.GetService<IODataEntitySetsKeyChildBatchHandler<TChildViewModel, TKey, TChildViewModel, TChildKey>>();
        if (handler == null)
        {
            return NotFound();
        }

        var resut = await handler.PatchModel(new PatchEntitySetKeyChildSetCommand<TChildViewModel, TKey, TChildViewModel, TChildKey>
        {
            ChildKey = childKey,
            Key = key,
            Delta = delta
        }, cancellationToken);

        return resut.ToActionResult();
    }
}
