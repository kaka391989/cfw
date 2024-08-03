using CFW.Core.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using CFW.OData.Commands;
using CFW.OData.Conventions;
using CFW.OData.Handlers;

namespace CFW.OData.Controllers
{
    [ODataEntitySetActionsConvention]
    internal class ODataEntitySetActionsController<TViewModel, TKey, TBody, TReturn> : ODataController
        where TViewModel : class
    {
        [HttpPost]
        public async Task<IActionResult> PostAction([FromODataUri] TKey key, [FromBody] TBody body,
            [FromServices] IServiceProvider serviceProvider,
            [FromServices] ValidationService validationService,
            CancellationToken cancellationToken
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var handler = serviceProvider.GetService<IODataEntitySetsBoundActionHandler<TViewModel, TKey, TBody, TReturn>>();

            if (handler == null)
            {
                return NotFound();
            }

            var command = new ActionCommand<TViewModel, TKey, TBody, TReturn> { Body = body, Key = key };

            var ruleDef = serviceProvider.GetService<IRuleDefinition<ActionCommand<TViewModel, TKey, TBody, TReturn>>>();
            if (ruleDef != null)
            {
                var validateResult = await validationService.Validate(ruleDef, command);
                if (validateResult != null && validateResult?.IsSuccess != true)
                {
                    return validateResult!.ToActionResult();
                }
            }

            var result = await handler.Execute(command, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        [EnableQuery]
        public async Task<IActionResult> PostUnboundAction(
            [FromBody] TBody body,
            [FromServices] IServiceProvider serviceProvider,
            [FromServices] ValidationService validationService,
            CancellationToken cancellationToken
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var handler = serviceProvider.GetService<IODataEntitySetsUnboundActionHandler<TViewModel, TKey, TBody, TReturn>>();
            if (handler == null)
            {
                return NotFound();
            }

            var command = new UnboundActionCommand<TViewModel, TKey, TBody, TReturn> { Body = body };
            var ruleDef = serviceProvider.GetService<IRuleDefinition<UnboundActionCommand<TViewModel, TKey, TBody, TReturn>>>();
            if (ruleDef != null)
            {
                var validateResult = await validationService.Validate(ruleDef, command);
                if (validateResult != null && validateResult?.IsSuccess != true)
                {
                    return validateResult!.ToActionResult();
                }
            }

            var result = await handler.Execute(command, cancellationToken);

            return result.ToActionResult();
        }
    }
}
