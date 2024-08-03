using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates
{
    internal class UnboundActionTemplate : ODataSegmentTemplate
    {
        private readonly IEdmEntitySet _edmEntitySet;
        private readonly IEdmAction _edmAction;

        public UnboundActionTemplate(IEdmEntitySet edmEntitySet, IEdmAction edmAction)
        {
            _edmEntitySet = edmEntitySet;
            _edmAction = edmAction;
        }

        public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
        {
            yield return $"/{_edmEntitySet.Name}/{_edmAction.Name}";
        }

        public override bool TryTranslate(ODataTemplateTranslateContext context)
        {
            var segment = new EntitySetSegment(_edmEntitySet);
            context.Segments.Add(segment);

            context.Segments.Add(new OperationSegment(_edmAction, null));
            context.RouteValues.Add("actionName", _edmAction.Name);
            return true;
        }
    }
}
