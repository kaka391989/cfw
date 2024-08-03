using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.NewtonsoftJson;
using System.Linq.Expressions;
using CFW.OData;

namespace CFW.OData.Extensions
{
    public static class OdataExtensions
    {
        public static IMvcBuilder AddGenericOData(this IMvcBuilder mvcBuilder)
        {
            var model = ODataContainer.Instance.Model;
            mvcBuilder.AddOData(opt =>
            {
                opt.EnableQueryFeatures();
                opt.AddRouteComponents(ODataContainer.RoutePrefix, model, services =>
                {
                    ODataContainer.Instance.RegisterSearchBinders(services);
                });
            }).AddODataNewtonsoftJson()
                .ConfigureApplicationPartManager(pm =>
                {
                    pm.FeatureProviders.Add(ODataContainer.Instance);
                });

            foreach (var handlerType in ODataContainer.Instance.Routes.SelectMany(x => x.HandlerTypes))
            {
                var interfaceTypes = handlerType
                        .GetInterfaces()
                        .Where(x => x.Name.StartsWith("IODataEntitySets") || x.Name.StartsWith("IODataSingleton"))
                        .ToList();

                // Adding base class interfaces
                var baseType = handlerType.BaseType;
                while (baseType != null && baseType != typeof(object))
                {
                    var baseInterfaces = baseType
                            .GetInterfaces()
                            .Where(x => x.Name.StartsWith("IODataEntitySets") || x.Name.StartsWith("IODataSingleton"));
                    interfaceTypes.AddRange(baseInterfaces);
                    baseType = baseType.BaseType;
                }

                foreach (var interfaceType in interfaceTypes.Distinct())
                {
                    mvcBuilder.Services.AddScoped(interfaceType, handlerType);
                }
            }

            return mvcBuilder;
        }

        public static WebApplication UseGenericOData(this WebApplication app)
        {
            app.UseRouting();
            app.MapControllers();
            //app.UseEndpoints(endpoints => endpoints.MapControllers());

            ODataContainer.Instance.RootService = app.Services;
            return app;
        }

        public static object? GetPropertyAccessor<TSource>(TSource source, string propertyName)
        {
            propertyName = char.ToUpper(propertyName[0]) + propertyName[1..];

            var par = Expression.Parameter(typeof(TSource), "x");
            var col = Expression.Property(par, propertyName);
            var accessProp = Expression.Lambda(col, par).Compile();
            var t = accessProp.DynamicInvoke(source);
            return t;
        }

    }
}
