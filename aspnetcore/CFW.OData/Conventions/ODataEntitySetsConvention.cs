using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using CFW.OData;
using CFW.OData.Controllers;
using CFW.OData.Extensions;
using CFW.OData.Models;
using CFW.OData.Templates;

namespace CFW.OData.Conventions
{
    internal class ODataEntitySetsConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType
                || controller.ControllerType.GetGenericTypeDefinition() != typeof(ODataEntitySetsController<,>))
            {
                return;
            }
            var odataRoute = ODataContainer.Instance.Routes.Single(x => x.ControllerTypes.Any(c => c == controller.ControllerType));
            controller.ControllerName = odataRoute.Route + "Controller";
            var model = ODataContainer.Instance.Model;
            var entitySet = model.EntityContainer.FindEntitySet(odataRoute.Route);

            if (entitySet == null)
            {
                throw new InvalidOperationException($"ControllerType {controller.ControllerType.FullName} is not a valid entity set.");
            }

            SetActionNonKey(nameof(ODataEntitySetsController<object, object>.GetEntitySets), controller, odataRoute, model, entitySet);
            SetActionNonKey(nameof(ODataEntitySetsController<object, object>.Create), controller, odataRoute, model, entitySet);

            SetActionWithKey(nameof(ODataEntitySetsController<object, object>.GetByKey), controller, odataRoute, model, entitySet);
            SetActionWithKey(nameof(ODataEntitySetsController<object, object>.Patch), controller, odataRoute, model, entitySet);
            SetActionWithKey(nameof(ODataEntitySetsController<object, object>.Delete), controller, odataRoute, model, entitySet);

            //GetPropertyValueByKey
            var getPropertyByKeyActionName = nameof(ODataEntitySetsController<object, object>.GetPropertyValueByKey);
            var actionModel = controller.Actions
                .Single(x => x.ActionName == getPropertyByKeyActionName);
            var template = new ODataPathTemplate(new EntitySetNavigationPropertyTemplate(entitySet));
            actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);

        }

        private void SetActionNonKey(string actionName, ControllerModel controller, ODataRoute odataRoute, IEdmModel model, IEdmEntitySet entitySet)
        {
            var actionModel = controller.Actions
                .Single(x => x.ActionName == actionName);

            var template = new ODataPathTemplate(new EntitySetsTemplate(entitySet));
            actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
        }

        private void SetActionWithKey(string actionName, ControllerModel controller, ODataRoute odataRoute, IEdmModel model, IEdmEntitySet entitySet)
        {
            var actionModel = controller.Actions
                .Single(x => x.ActionName == actionName);

            var template = new ODataPathTemplate(new EntitySetWithKeyTemplate(entitySet));
            actionModel.AddSelector(actionModel.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
            actionModel.RouteValues.Add("action", actionModel.ActionName);
        }
    }

}
