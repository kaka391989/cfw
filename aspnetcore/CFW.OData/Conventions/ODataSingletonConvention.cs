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
    public class ODataSingletonConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType
                            || controller.ControllerType.GetGenericTypeDefinition() != typeof(ODataSingletonController<>))
            {
                return;
            }

            var odataRoute = ODataContainer.Instance.Routes.Single(x => x.ControllerTypes.Any(c => c == controller.ControllerType));
            controller.ControllerName = odataRoute.Route + "Controller";
            var model = ODataContainer.Instance.Model;
            var singleton = model.EntityContainer.FindSingleton(odataRoute.Route);

            if (singleton == null)
            {
                throw new InvalidOperationException($"ControllerType {controller.ControllerType.FullName} is not a valid singleton.");
            }


            //GET
            var getActionName = nameof(ODataSingletonController<object>.Get);
            var actionModel = controller.Actions.Single(x => x.ActionName == getActionName);
            var template = new ODataPathTemplate(new SingletonTemplate(singleton));

            actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
            actionModel.RouteValues.Add("action", actionModel.ActionName);

            //GET property
            var getPropertyActionName = nameof(ODataSingletonController<object>.GetProperty);
            actionModel = controller.Actions.Single(x => x.ActionName == getPropertyActionName);
            template = new ODataPathTemplate(new SingletonNavigationPropertyTemplate(singleton));

            actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
            actionModel.RouteValues.Add("action", actionModel.ActionName);

            //PATCH
            var patchActionName = nameof(ODataSingletonController<object>.Patch);
            actionModel = controller.Actions.Single(x => x.ActionName == patchActionName);
            template = new ODataPathTemplate(new SingletonTemplate(singleton));

            actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
            actionModel.RouteValues.Add("action", actionModel.ActionName);

            //ACTION
            var singletonActionName = nameof(ODataSingletonController<object>.Action);
            actionModel = controller.Actions.Single(x => x.ActionName == singletonActionName);

            var singletonFullName = singleton.EntityType().FullName();
            var interfaceType = typeof(IODataSingletonActionHandler<>);
            var handlerInterfaceType = interfaceType.MakeGenericType(controller.ControllerType.GetGenericArguments());

            var handlerType = odataRoute
                .HandlerTypes
                .FirstOrDefault(x => x.GetInterfaces().Any(i => i == handlerInterfaceType));

            if (handlerType != null)
            {

                var handlerAction = handlerType.GetCustomAttribute<ODataRoutingAttribute>()!.Action;

                var edmActions = model.SchemaElements
                                    .OfType<IEdmAction>()
                                    .Where(x => x.Parameters.First().Type.FullName() == singletonFullName)
                                    .Where(x => x.Name == handlerAction)
                                    .ToList();

                if (edmActions.Any())
                {
                    template = new ODataPathTemplate(new SingletonActionTemplate(singleton, edmActions));
                    actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
                }
            }
        }
    }
}
