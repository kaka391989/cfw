using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using CFW.OData;
using CFW.OData.Controllers;
using CFW.OData.Extensions;
using CFW.OData.Templates;

namespace CFW.OData.Conventions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal class ODataEntitySetKeyChildSetConvention : Attribute, IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (!controller.ControllerType.IsGenericType
            || controller.ControllerType.GetGenericTypeDefinition() != typeof(ODataEntitySetKeyChildSetsController<,,,>))
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

        var postMethodName = nameof(ODataEntitySetKeyChildSetsController<object, object, object, object>.PostChild);
        var postAction = controller.Actions.SingleOrDefault(x => x.ActionName == postMethodName);
        if (postAction != null)
        {
            var childSetType = controller.ControllerType.GetGenericArguments()[2];

            var childEntitySet = model.EntityContainer.EntitySets()
                .SingleOrDefault(es => es.EntityType().FullName() == childSetType.FullName);

            if (childEntitySet == null)
            {
                throw new InvalidOperationException("Child entity set not found.");
            }

            var template = new ODataPathTemplate(new EntitySetEntityChildTemplate(entitySet, childEntitySet));
            postAction.AddSelector(postAction.GetHttpMethodName(), odataRoute.RoutePrefix, model, template);
        }
    }
}
