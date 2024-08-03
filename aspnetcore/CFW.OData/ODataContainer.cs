using CFW.OData.Attributes;
using CFW.OData.Handlers;
using CFW.OData.Models;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Reflection;

namespace CFW.OData
{
    public class ODataContainer : IApplicationFeatureProvider<ControllerFeature>
    {
        private static readonly object _padlock = new object();
        private static ODataContainer? _instance;
        public static ODataContainer Instance
        {
            get
            {
                lock (_padlock)
                {
                    _instance ??= new ODataContainer();
                    return _instance;
                }
            }
        }

        private static readonly string _namespacePrefix = Assembly.GetEntryAssembly()!.FullName!.Split(".")[0];

        public const string RoutePrefix = "odata";

        public ODataContainer()
        {
            Routes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && x.FullName != null && x.FullName.StartsWith(_namespacePrefix))
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetCustomAttribute<ODataRoutingAttribute>() != null)
                .Select(x => new { HandlerType = x, x.GetCustomAttribute<ODataRoutingAttribute>()!.Name })
                .GroupBy(x => x!.Name)
                .Select(x => new ODataRoute(RoutePrefix, x.Key) { HandlerTypes = x.Select(x => x.HandlerType).ToList() })
                .ToList();

            Model = CreateModel();
        }

        public IEdmModel Model { private set; get; }

        internal List<ODataRoute> Routes { private set; get; }

        public IServiceProvider? RootService { get; set; }

        private IEdmModel CreateModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();

            foreach (var odataRoute in Routes)
            {
                odataRoute.AddEntry(modelBuilder, _namespacePrefix);
            }

            modelBuilder.EnableLowerCamelCase();
            modelBuilder.Namespace = "CFW";
            return modelBuilder.GetEdmModel();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            Routes
                .SelectMany(x => x.ControllerTypes.Select(c => c.GetTypeInfo()))
                .ToList()
                .ForEach(x => feature.Controllers.Add(x));
        }

        internal void RegisterSearchBinders(IServiceCollection services)
        {
            var searchableQueryTypes = Routes
            .SelectMany(x => x.HandlerTypes)
            .Where(x => x.GetInterfaces()
                         .Any(i => i.IsGenericType &&
                                   i.GetGenericTypeDefinition() == typeof(ISearchableQuery<>)))
            .ToList();

            foreach (var searchableQueryType in searchableQueryTypes)
            {
                var searchableQueryInterfaceType = searchableQueryType.GetInterfaces()
                    .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISearchableQuery<>));
                var viewModelType = searchableQueryInterfaceType.GetGenericArguments()[0];
                var searchBinder = typeof(SearchBinder<>).MakeGenericType(viewModelType);
                services.AddSingleton(
                    typeof(ISearchBinder),
                    s =>
                    {
                        return ActivatorUtilities.CreateInstance(s, searchBinder, searchableQueryType);
                    }
                );
            }
        }
    }
}
