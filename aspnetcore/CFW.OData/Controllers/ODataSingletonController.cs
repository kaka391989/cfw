using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using CFW.OData.Conventions;
using CFW.OData.Extensions;

namespace CFW.OData.Controllers
{
    //Handle Get, GetList, Post, Patch, Delete of OData singletons.
    [ODataSingletonConvention]
    internal class ODataSingletonController<TViewModel> : ODataController where TViewModel : class
    {
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
            => await this.HandleGetSingleton(cancellationToken);

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetProperty([FromRoute] string propertyName, CancellationToken cancellationToken)
            => await this.HandleGetSingletonProperty(propertyName, cancellationToken);

        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] Delta<TViewModel> delta, CancellationToken cancellationToken)
            => await this.HandleSingletonPatch(delta, cancellationToken);

        [HttpPost]
        public async Task<IActionResult> Action([FromRoute] string actionName, CancellationToken cancellationToken)
            => await this.HandleSingletonAction(actionName, cancellationToken);
    }
}
