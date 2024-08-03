using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class NestedEntitySetTemplate : ODataSegmentTemplate
{
    private readonly string _setRoute;
    private readonly string _childRoute;
    private readonly string _propertySetName;

    public NestedEntitySetTemplate(string setRoute, string childRoute, string propertySetName)
    {
        _setRoute = setRoute;
        _childRoute = childRoute;
        _propertySetName = propertySetName;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        yield return $"/{_setRoute}/{{key}}/{_childRoute}/{{childKey}}";
        yield return $"/{_setRoute}({{key}})/{_childRoute}({{childKey}})";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var entitySet = context
            .Model.EntityContainer.EntitySets()
            .FirstOrDefault(x => string.Equals(x.Name, _setRoute, StringComparison.OrdinalIgnoreCase));

        if (entitySet != null)
        {
            EntitySetSegment parentSegment = new EntitySetSegment(entitySet);
            context.Segments.Add(parentSegment);

            if (context.RouteValues.TryGetValue("key", out object? parentKeyValue))
            {
                var entitySetType = parentSegment.EntitySet.EntityType();
                var parentKey = entitySetType.DeclaredKey.First();
                KeySegment parentKeySegment = new KeySegment(
                    new Dictionary<string, object> { { parentKey.Name, parentKeyValue! } },
                    entitySetType,
                    entitySet
                );
                context.Segments.Add(parentKeySegment);

                if (context.RouteValues.TryGetValue("childKey", out object? childKeyValue))
                {
                    var childEntitySet = context.Model.EntityContainer.EntitySets().FirstOrDefault(x => x.Name == _propertySetName);

                    if (childEntitySet != null)
                    {
                        EntitySetSegment childSegment = new EntitySetSegment(childEntitySet);
                        context.Segments.Add(childSegment);

                        var childEntitySetType = childSegment.EntitySet.EntityType();
                        var childKey = childEntitySetType.DeclaredKey.First();
                        KeySegment childKeySegment = new KeySegment(
                            new Dictionary<string, object> { { childKey.Name, childKeyValue! } },
                            childEntitySetType,
                            childEntitySet
                        );
                        context.Segments.Add(childKeySegment);

                        return true;
                    }
                }
            }
        }

        return false;
    }
}
