using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class EntitySetEntityChildTemplate : ODataSegmentTemplate
{
    private readonly IEdmEntitySet _edmEntitySet;
    private readonly IEdmEntitySet _edmEntityChild;

    public EntitySetEntityChildTemplate(IEdmEntitySet edmEntitySet, IEdmEntitySet edmEntityChild)
    {
        _edmEntitySet = edmEntitySet;
        _edmEntityChild = edmEntityChild;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        var nav = _edmEntitySet.NavigationPropertyBindings.Where(x => x.Target == _edmEntityChild)
            .Select(x => x.NavigationProperty.Name).First();

        yield return $"{_edmEntitySet.Name}({{key}})/{nav}";
        yield return $"{_edmEntitySet.Name}/{{key}}/{nav}";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var entitySetSegment = new EntitySetSegment(_edmEntitySet);
        var entityType = _edmEntitySet.EntityType();

        if (!context.RouteValues.TryGetValue("key", out var key))
        {
            throw new InvalidOperationException("Failed to get key value.");
        }

        var keySegment = new KeySegment(new[] { new KeyValuePair<string, object>("Id", key!) }, entityType, _edmEntitySet);
        context.Segments.Add(entitySetSegment);
        context.Segments.Add(keySegment);

        var entitySetSegmentChild = new EntitySetSegment(_edmEntityChild);
        context.Segments.Add(entitySetSegmentChild);

        return true;
    }
}
