using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CFW.OData.Templates
{
    internal class EntitySetWithKeyTemplate : ODataSegmentTemplate
    {
        private readonly IEdmEntitySet _edmEntitySet;

        public EntitySetWithKeyTemplate(IEdmEntitySet edmEntitySet)
        {
            _edmEntitySet = edmEntitySet;
        }

        public override IEnumerable<string> GetTemplates(ODataRouteOptions options)
        {
            yield return $"/{_edmEntitySet.Name}/{{key}}";
            yield return $"/{_edmEntitySet.Name}({{key}})";
        }

        public override bool TryTranslate(ODataTemplateTranslateContext context)
        {
            if (!context.RouteValues.TryGetValue("key", out object? value))
            {
                return false;
            }

            var segment = new EntitySetSegment(_edmEntitySet);
            context.Segments.Add(segment);

            var key = segment.EntitySet.EntityType().DeclaredKey.First();
            var keySegment = new KeySegment(new Dictionary<string, object> { { key.Name, value! } }, _edmEntitySet.EntityType(), _edmEntitySet);
            context.Segments.Add(keySegment);

            return true;
        }
    }
}
