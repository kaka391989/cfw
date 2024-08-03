using CFW.Core.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using System.Reflection;
using CFW.OData.Attributes;
using CFW.OData.Commands;
using CFW.OData.Controllers;
using CFW.OData.Extensions;
using CFW.OData.Handlers;

namespace CFW.OData.Extensions
{
    public static class ODataSingletonControllerExtensions
    {
        internal static async Task<IActionResult> HandleGetSingleton<TViewModel>(
            this ODataSingletonController<TViewModel> controller, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataSingletonQueryHandler<TViewModel>>();

            if (handler == null)
            {
                return controller.NotFound();
            }

            var result = await handler.GetModel(cancellationToken);
            return controller.Ok(result);
        }

        internal static async Task<IActionResult> HandleGetSingletonProperty<TViewModel>(
            this ODataSingletonController<TViewModel> controller, string propertyName, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataSingletonQueryHandler<TViewModel>>();

            if (handler == null)
            {
                return controller.NotFound();
            }

            var model = await handler.GetModel(cancellationToken);
            var result = OdataExtensions.GetPropertyAccessor(model, propertyName);
            return controller.Ok(result);
        }

        internal static async Task<IActionResult> HandleSingletonPatch<TViewModel>(
            this ODataSingletonController<TViewModel> controller, Delta<TViewModel> delta, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataSingletonPatchHandler<TViewModel>>();

            if (handler == null)
            {
                return controller.NotFound();
            }

            var command = new BatchSingleCommand<TViewModel> { Delta = delta };
            var ruleDefinition = serviceProvider.GetService<IRuleDefinition<BatchSingleCommand<TViewModel>>>();
            var validationService = serviceProvider.GetRequiredService<ValidationService>();
            if (ruleDefinition != null)
            {
                var validationResult = await validationService.Validate(ruleDefinition, command);

                if (validationResult != null && !validationResult.IsSuccess)
                {
                    return validationResult.ToActionResult();
                }
            }

            var result = await handler!.PatchSingleModel(command, cancellationToken);
            return result.ToActionResult();
        }

        internal static async Task<IActionResult> HandleSingletonAction<TViewModel>(
            this ODataSingletonController<TViewModel> controller, string actionName, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handlers = serviceProvider.GetServices<IODataSingletonActionHandler<TViewModel>>();

            if (handlers == null)
            {
                return controller.NotFound();
            }

            var handler = handlers.FirstOrDefault(x => x.GetType().GetCustomAttributes<ODataRoutingAttribute>().Any(a => a.Action == actionName));

            if (handler == null)
            {
                return controller.NotFound();
            }


            var result = await handler.Execute(cancellationToken);
            return result.ToActionResult();
        }
    }
}
