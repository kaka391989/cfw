using CFW.Core.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CFW.OData.Attributes;
using CFW.OData.Handlers;
using ChubbLife.Api.DbModels;

namespace ChubbLife.Api.Features.Core
{
    public class LocalesQueryModel
    {
        [Key]
        public string Locale { get; set; } = string.Empty;

        public object Resources { get; set; } = new Dictionary<string, string>();
    }

    [ODataRouting(Name = "locales")]
    public class LocalesQuery : IODataEntitySetsQueryHandler<LocalesQueryModel, string>
    {
        public async Task<LocalesQueryModel?> GetModel(string key, CancellationToken cancellationToken)
        {
            var displayNames = RefectionUtils.GetDisplayNames<Employee>();
            return await Task.FromResult(new LocalesQueryModel
            {
                Locale = key,
                Resources = displayNames.ToJsonWithCamelCase()
            });
        }

        public Task<IQueryable<LocalesQueryModel>> GetQuery(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static List<(string PropertyName, string DisplayName)> GetDisplayNames<T>()
        {
            var result = new List<(string PropertyName, string DisplayName)>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var displayNameAttribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                                   .FirstOrDefault() as DisplayNameAttribute;

                var displayName = displayNameAttribute != null ? displayNameAttribute.DisplayName : property.Name;

                result.Add((property.Name, displayName));
            }

            return result;
        }
    }
}
