using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.UriParser;
using System.Linq.Expressions;
using CFW.OData;
using CFW.OData.Handlers;

namespace CFW.OData.Handlers
{
    public class SearchBinder<TViewModel> : ISearchBinder
    {
        private readonly ISearchableQuery<TViewModel> _searchableQuery;

        public SearchBinder(Type implementationType)
        {
            using var scope = ODataContainer.Instance.RootService!.CreateScope();
            var implementation = ActivatorUtilities.CreateInstance(scope.ServiceProvider, implementationType);

            if (implementation is not ISearchableQuery<TViewModel> searchableQuery)
            {
                throw new InvalidOperationException(
                                       $"The type {implementationType} does not implement {typeof(ISearchableQuery<TViewModel>)}");
            }

            _searchableQuery = searchableQuery;
        }

        public Expression BindSearch(SearchClause searchClause, QueryBinderContext context)
        {
            var node = searchClause.Expression as SearchTermNode;
            var text = node!.Text.Trim();
            var exp = _searchableQuery.GetSearchExpression(text);
            return exp;
        }
    }
}
