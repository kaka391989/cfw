using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class SingletonActionTemplate : ODataSegmentTemplate
{
    private readonly IEdmSingleton _edmSingleton;
    private readonly IEnumerable<IEdmAction> _edmActions;

    public SingletonActionTemplate(IEdmSingleton edmSingleton, IEnumerable<IEdmAction> edmActions)
    {
        _edmSingleton = edmSingleton;
        _edmActions = edmActions;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        foreach (var action in _edmActions)
        {
            yield return $"/{_edmSingleton.Name}/{action.Name}";
        }
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var segment = new SingletonSegment(_edmSingleton);
        context.Segments.Add(segment);

        var actionName = context.HttpContext.Request.Path.Value!.Split('/').Last();
        var edmAction = _edmActions
            .Where(x => string.Equals(x.Name, actionName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault(x => x.Parameters.Single().Type.Definition.FullTypeName()
                == _edmSingleton!.Type.AsElementType().FullTypeName());

        if (edmAction == null)
        {
            return false;
        }

        context.Segments.Add(new OperationSegment(edmAction, null));
        context.RouteValues.Add("actionName", edmAction.Name);

        return true;
    }
}
