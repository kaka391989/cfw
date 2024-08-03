using System.Linq.Expressions;

namespace CFW.OData.Handlers
{
    public interface ISearchableQuery<TViewModel>
    {
        Expression<Func<TViewModel, bool>> GetSearchExpression(string searchText);
    }
}
