using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class SingletonTemplate : ODataSegmentTemplate
{
    private readonly IEdmSingleton _edmSingleton;

    public SingletonTemplate(IEdmSingleton edmSingleton)
    {
        _edmSingleton = edmSingleton;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        yield return $"/{_edmSingleton.Name}";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var segment = new SingletonSegment(_edmSingleton);
        context.Segments.Add(segment);
        return true;
    }
}
