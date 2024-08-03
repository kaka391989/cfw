using CFW.Core.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using CFW.OData.Commands;
using CFW.OData.Conventions;
using CFW.OData.Handlers;

namespace CFW.OData.Controllers
{
    [ODataEntitySetActionsNotReturnConvention]
    internal class ODataEntitySetActionsReturnEmptyController<TViewModel, TKey, TBody> : ODataController
        where TViewModel : class
    {
        [HttpPost]
        public async Task<IActionResult> PostAction([FromODataUri] TKey key, [FromODataBody] TBody body,
            [FromServices] IServiceProvider serviceProvider,
            [FromServices] ValidationService validationService,
            CancellationToken cancellationToken
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var handler = serviceProvider.GetService<IODataEntitySetsBoundActionHandler<TViewModel, TKey, TBody>>();

            if (handler == null)
            {
                return NotFound();
            }

            var command = new ActionCommand<TViewModel, TKey, TBody> { Body = body, Key = key };

            var ruleDef = serviceProvider.GetService<IRuleDefinition<ActionCommand<TViewModel, TKey, TBody>>>();
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
        public async Task<IActionResult> PostUnboundAction([FromODataBody] TBody body,
                [FromServices] IServiceProvider serviceProvider,
                [FromServices] ValidationService validationService,
                CancellationToken cancellationToken
            )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var handler = serviceProvider.GetService<IODataEntitySetsUnboundActionHandler<TViewModel, TKey, TBody>>();

            if (handler == null)
            {
                return NotFound();
            }

            var command = new UnboundActionCommand<TViewModel, TKey, TBody> { Body = body };

            var ruleDef = serviceProvider.GetService<IRuleDefinition<UnboundActionCommand<TViewModel, TKey, TBody>>>();
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
