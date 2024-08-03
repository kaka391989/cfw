using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class EntitySetsTemplate : ODataSegmentTemplate
{
    private readonly IEdmEntitySet _edmEntitySet;

    public EntitySetsTemplate(IEdmEntitySet edmEntitySet)
    {
        _edmEntitySet = edmEntitySet;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        yield return $"/{_edmEntitySet.Name}";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var segment = new EntitySetSegment(_edmEntitySet);
        context.Segments.Add(segment);
        return true;
    }
}
