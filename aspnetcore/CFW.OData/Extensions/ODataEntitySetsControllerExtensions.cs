using CFW.Core.Services.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using CFW.OData.Commands;
using CFW.OData.Controllers;
using CFW.OData.Extensions;
using CFW.OData.Handlers;

namespace CFW.OData.Extensions
{
    internal static class ODataEntitySetsControllerExtensions
    {
        internal static async Task<IActionResult> HandleGetEntitySets<TViewModel, TKey>(
            this ODataEntitySetsController<TViewModel, TKey> controller,
            ODataQueryOptions<TViewModel> options,
            CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataEntitySetsQueryHandler<TViewModel, TKey>>();

            if (handler != null)
            {
                var result = await handler.GetQuery(cancellationToken);
                return controller.Ok(result);
            }

            var customHandler = serviceProvider.GetService<IODataEntitySetsCustomQueryHandler<TViewModel, TKey>>();
            if (customHandler != null)
            {
                var result = await customHandler.GetQuery(options, cancellationToken);
                return controller.Ok(result);
            }

            return controller.NotFound();
        }

        internal static async Task<IActionResult> HandleGetEntitySetByKey<TViewModel, TKey>(
            this ODataEntitySetsController<TViewModel, TKey> controller, TKey key, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataEntitySetsQueryHandler<TViewModel, TKey>>();

            if (handler == null)
            {
                return controller.NotFound();
            }

            var result = await handler.GetModel(key, cancellationToken);
            return controller.Ok(result);
        }

        internal static async Task<IActionResult> CreateEntitySet<TViewModel, TKey>(
            this ODataEntitySetsController<TViewModel, TKey> controller, TViewModel viewModel, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataEntitySetsCreateHandler<TViewModel, TKey>>();

            if (handler == null)
            {
                return controller.NotFound();
            }

            var command = new CreateCommand<TViewModel, TKey>
            {
                Model = viewModel
            };

            var validationService = serviceProvider.GetRequiredService<ValidationService>();
            var ruleDefinition = serviceProvider.GetService<IRuleDefinition<CreateCommand<TViewModel, TKey>>>();
            if (ruleDefinition != null)
            {
                var validationResult = await validationService.Validate(ruleDefinition, command);

                if (validationResult != null && !validationResult.IsSuccess)
                {
                    return validationResult.ToActionResult();
                }
            }

            var result = await handler.CreateModel(command, cancellationToken);

            return result.ToActionResult();
        }

        internal static async Task<IActionResult> PatchEntitySet<TViewModel, TKey>(
            this ODataEntitySetsController<TViewModel, TKey> controller, TKey key, Delta<TViewModel> delta
            , CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataEntitySetsBatchHandler<TViewModel, TKey>>();

            if (handler == null)
            {
                return controller.NotFound();
            }

            var command = new PatchCommand<TViewModel, TKey>
            {
                Key = key,
                Delta = delta
            };

            var validationService = serviceProvider.GetRequiredService<ValidationService>();
            var ruleDefinition = serviceProvider.GetService<IRuleDefinition<PatchCommand<TViewModel, TKey>>>();
            if (ruleDefinition != null)
            {
                var validationResult = await validationService.Validate(ruleDefinition, command);

                if (validationResult != null && !validationResult.IsSuccess)
                {
                    return validationResult.ToActionResult();
                }
            }

            var result = await handler.PatchModel(command, cancellationToken);

            return result.ToActionResult();
        }

        internal static async Task<IActionResult> DeleteEntitySet<TViewModel, TKey>(
            this ODataEntitySetsController<TViewModel, TKey> controller, TKey key, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataEntitySetsDeleteHandler<TViewModel, TKey>>();

            if (handler == null)
            {
                return controller.NotFound();
            }

            var command = new DeleteCommand<TViewModel, TKey>
            {
                Key = key
            };

            var validationService = serviceProvider.GetRequiredService<ValidationService>();
            var ruleDefinition = serviceProvider.GetService<IRuleDefinition<DeleteCommand<TViewModel, TKey>>>();
            if (ruleDefinition != null)
            {
                var validationResult = await validationService.Validate(ruleDefinition, command);

                if (validationResult != null && !validationResult.IsSuccess)
                {
                    return validationResult.ToActionResult();
                }
            }

            var result = await handler.DeleteModel(command, cancellationToken);

            return result.ToActionResult();
        }

        internal static async Task<IActionResult> HandleGetPropertyValueByKey<TViewModel, TKey>(
            this ODataEntitySetsController<TViewModel, TKey> controller, TKey key
            , string propertyName, ODataQueryOptions<TViewModel> options, CancellationToken cancellationToken)
            where TViewModel : class
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.BadRequest(controller.ModelState);
            }

            var serviceProvider = controller.HttpContext.RequestServices;
            var handler = serviceProvider.GetService<IODataEntitySetsQueryHandler<TViewModel, TKey>>();

            if (handler is not null)
            {
                if (handler is IODataEntitySetsGetPropertyByKeyHandler<TViewModel, TKey> propertyHandler)
                {
                    var result = await propertyHandler.GetProperty(key, propertyName, cancellationToken);
                    if (result is null)
                    {
                        return controller.NotFound();
                    }
                    return controller.Ok(result);
                }
                else
                {
                    var entity = await handler!.GetModel(key, cancellationToken);
                    var propertyValue = OdataExtensions.GetPropertyAccessor(entity, propertyName);

                    return controller.Ok(propertyValue);
                }
            }

            var customHandler = serviceProvider.GetService<IODataEntitySetsCustomQueryHandler<TViewModel, TKey>>();
            if (customHandler != null && customHandler is IODataEntitySetsGetPropertyByKeyHandler<TViewModel, TKey> customPropertyHandler)
            {
                var result = await customPropertyHandler.GetProperty(key, propertyName, cancellationToken);
                if (result is null)
                {
                    return controller.NotFound();
                }
                return controller.Ok(result);
            }

            return controller.NotFound();

        }

    }
}
