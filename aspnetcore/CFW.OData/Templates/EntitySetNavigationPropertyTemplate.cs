using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class EntitySetNavigationPropertyTemplate : ODataSegmentTemplate
{
    private readonly IEdmEntitySet _edmEntitySet;

    public EntitySetNavigationPropertyTemplate(IEdmEntitySet edmEntitySet)
    {
        _edmEntitySet = edmEntitySet;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        yield return $"{_edmEntitySet.Name}({{key}})/{{navigationProperty}}";
        yield return $"{_edmEntitySet.Name}/{{key}}/{{navigationProperty}}";
        yield return $"{_edmEntitySet.Name}({{key}})/{{navigationProperty}}/$value";
        yield return $"{_edmEntitySet.Name}/{{key}}/{{navigationProperty}}/$value";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        IEdmModel model = context.Model;
        var entitySetSegment = new EntitySetSegment(_edmEntitySet);
        var entityType = _edmEntitySet.EntityType();

        if (!context.RouteValues.TryGetValue("key", out var key))
        {
            throw new NotImplementedException("Get key value falled.");
        }

        var keySegment = new KeySegment(new[] { new KeyValuePair<string, object>("Id", key!) }, entityType, _edmEntitySet);

        context.RouteValues.TryGetValue("navigationProperty", out var navigationProperty);

        context.Segments.Add(entitySetSegment);
        context.Segments.Add(keySegment);


        var propertyName = string.Empty;
        var edmStructualProperty = _edmEntitySet
            .EntityType()
            .StructuralProperties()
            .FirstOrDefault(c => c.Name.Equals(navigationProperty!.ToString(), StringComparison.OrdinalIgnoreCase));

        if (edmStructualProperty is not null)
        {
            var navSegment = new PropertySegment(edmStructualProperty);
            context.Segments.Add(navSegment);
            if (context.HttpContext.Request.Path.Value?.EndsWith("/$value") == true)
            {
                context.Segments.Add(new ValueSegment(navSegment.EdmType));
            }
            propertyName = edmStructualProperty.Name;
        }
        else
        {
            var edmNavigation = _edmEntitySet
                .EntityType()
                .NavigationProperties()
                .FirstOrDefault(c => c.Name.Equals(navigationProperty!.ToString(), StringComparison.OrdinalIgnoreCase));

            if (edmNavigation is null)
            {
                return false;
            }

            var navSegment = new NavigationPropertySegment(edmNavigation, null);
            context.Segments.Add(navSegment);

            propertyName = edmNavigation!.Name;
        }

        context.RouteValues.Add("PropertyName", UpperFirstChar(propertyName));
        return true;
    }

    public static string UpperFirstChar(string source)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        return char.ToUpper(source[0]) + source[1..];
    }
}
