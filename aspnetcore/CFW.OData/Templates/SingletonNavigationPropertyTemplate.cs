using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class SingletonNavigationPropertyTemplate : ODataSegmentTemplate
{
    private readonly IEdmSingleton _singleton;

    public SingletonNavigationPropertyTemplate(IEdmSingleton singleton)
    {
        _singleton = singleton;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        yield return $"{_singleton.Name}/{{navigationProperty}}";
        yield return $"{_singleton.Name}/{{navigationProperty}}";
        yield return $"{_singleton.Name}/{{navigationProperty}}/$value";
        yield return $"{_singleton.Name}/{{navigationProperty}}/$value";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var model = context.Model;
        var singletonSegment = new SingletonSegment(_singleton);

        context.RouteValues.TryGetValue("navigationProperty", out var navigationProperty);

        var edmNavProperty = _singleton.EntityType()
            .StructuralProperties()
            .FirstOrDefault(c => c.Name.Equals(navigationProperty!.ToString(), StringComparison.OrdinalIgnoreCase));

        var navSegment = new PropertySegment(edmNavProperty);

        context.Segments.Add(singletonSegment);
        context.Segments.Add(navSegment);

        if (context.HttpContext.Request.Path.Value?.EndsWith("/$value") == true)
        {
            context.Segments.Add(new ValueSegment(navSegment.EdmType));
        }

        context.RouteValues.Add("PropertyName", edmNavProperty!.Name);
        return true;

    }
}
