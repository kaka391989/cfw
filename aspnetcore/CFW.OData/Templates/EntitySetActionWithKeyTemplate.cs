using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates;

internal class EntitySetActionWithKeyTemplate : ODataSegmentTemplate
{
    private readonly IEdmEntitySet _edmEntitySet;
    private readonly IEdmAction _edmAction;

    public EntitySetActionWithKeyTemplate(IEdmEntitySet edmEntitySet, IEdmAction edmAction)
    {
        _edmEntitySet = edmEntitySet;
        _edmAction = edmAction;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        yield return $"{_edmEntitySet.Name}({{key}})/{_edmAction.Name}";
        yield return $"{_edmEntitySet.Name}/{{key}}/{_edmAction.Name}";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var entitySetSegment = new EntitySetSegment(_edmEntitySet);
        var entityType = _edmEntitySet.EntityType();

        if (!context.RouteValues.TryGetValue("key", out var key))
        {
            throw new NotImplementedException("Get key value falled.");
        }

        var keySegment = new KeySegment(new[] { new KeyValuePair<string, object>("Id", key!) }, entityType, _edmEntitySet);

        context.Segments.Add(entitySetSegment);
        context.Segments.Add(keySegment);

        context.Segments.Add(new OperationSegment(_edmAction, null));
        context.RouteValues.Add("actionName", _edmAction.Name);
        return true;

    }
}

internal class EntitySetActionWithoutKeyTemplate : ODataSegmentTemplate
{
    private readonly IEdmEntitySet _edmEntitySet;
    private readonly IEdmAction _edmAction;

    public EntitySetActionWithoutKeyTemplate(IEdmEntitySet edmEntitySet, IEdmAction edmAction)
    {
        _edmEntitySet = edmEntitySet;
        _edmAction = edmAction;
    }

    public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
    {
        yield return $"{_edmEntitySet.Name}/{_edmAction.Name}";
    }

    public override bool TryTranslate(ODataTemplateTranslateContext context)
    {
        var entitySetSegment = new EntitySetSegment(_edmEntitySet);

        context.Segments.Add(entitySetSegment);

        context.Segments.Add(new OperationSegment(_edmAction, null));
        context.RouteValues.Add("actionName", _edmAction.Name);
        return true;

    }
}
