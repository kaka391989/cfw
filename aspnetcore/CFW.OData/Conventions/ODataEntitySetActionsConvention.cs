using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using System.Reflection;
using CFW.OData;
using CFW.OData.Attributes;
using CFW.OData.Controllers;
using CFW.OData.Extensions;
using CFW.OData.Handlers;
using CFW.OData.Templates;

namespace CFW.OData.Conventions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class ODataEntitySetActionsConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (
                !controller.ControllerType.IsGenericType
                || controller.ControllerType.GetGenericTypeDefinition() != typeof(ODataEntitySetActionsController<,,,>)
            )
            {
                return;
            }

            var actionHandlers = new Dictionary<string, Action<string, ControllerModel>>
            {
                { nameof(ODataEntitySetActionsController<object, Guid, object, object>.PostAction), HandleBoundedAction },
                { nameof(ODataEntitySetActionsController<object, Guid, object, object>.PostUnboundAction), HandleUnboundAction }
            };

            foreach (var actionHandler in actionHandlers)
            {
                actionHandler.Value(actionHandler.Key, controller);
            }
        }

        private void HandleBoundedAction(string actionName, ControllerModel controller)
        {
            var model = ODataContainer.Instance.Model;
            var actionModel = controller.Actions.Single(x => x.ActionName == actionName);

            var odataRoute = ODataContainer.Instance.Routes.Single(x => x.ControllerTypes.Any(c => c == controller.ControllerType));

            var entitySet = model.EntityContainer.FindEntitySet(odataRoute.Route);
            var entitySetFullName = entitySet.EntityType().FullName();

            var interfaceType = typeof(IODataEntitySetsBoundActionHandler<,,,>);
            var handlerInterfaceType = interfaceType.MakeGenericType(controller.ControllerType.GetGenericArguments());

            var handlerType = odataRoute
                .HandlerTypes
                .SingleOrDefault(x => x.GetInterfaces().Any(i => i == handlerInterfaceType));

            if (handlerType is null)
            {
                return;
            }

            var handlerAction = handlerType.GetCustomAttribute<ODataRoutingAttribute>()!.Action;

            var edmAction = model.SchemaElements
                                        .OfType<IEdmAction>()
                                        .Where(x => x.Parameters.Count() == 1)
                                        .Where(x => x.Parameters.First().Type.FullName() == entitySetFullName)
                                        .SingleOrDefault(x => x.Name == handlerAction);

            if (edmAction is not null)
            {
                var template = new ODataPathTemplate(new EntitySetActionWithKeyTemplate(entitySet, edmAction));
                actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
            }

        }

        private void HandleUnboundAction(string actionName, ControllerModel controller)
        {
            var model = ODataContainer.Instance.Model;
            var actionModel = controller.Actions.Single(x => x.ActionName == actionName);

            var odataRoute = ODataContainer.Instance.Routes.Single(x => x.ControllerTypes.Any(c => c == controller.ControllerType));

            var entitySet = model.EntityContainer.FindEntitySet(odataRoute.Route);
            var entitySetFullName = entitySet.EntityType().FullName();

            var interfaceType = typeof(IODataEntitySetsUnboundActionHandler<,,,>);
            var handlerInterfaceType = interfaceType.MakeGenericType(controller.ControllerType.GetGenericArguments());

            var handlerType = odataRoute
                .HandlerTypes
                .SingleOrDefault(x => x.GetInterfaces().Any(i => i == handlerInterfaceType));

            if (handlerType is null)
            {
                return;
            }

            var handlerAction = handlerType.GetCustomAttribute<ODataRoutingAttribute>()!.Action;

            var edmAction = model.SchemaElements
                                        .OfType<IEdmAction>()
                                        .Where(x => x.Parameters.First().Type.FullName() == entitySetFullName)
                                        .FirstOrDefault(x => x.Name == handlerAction);

            if (edmAction is not null)
            {
                var template = new ODataPathTemplate(new EntitySetActionWithoutKeyTemplate(entitySet, edmAction));
                actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
            }
        }
    }
}
