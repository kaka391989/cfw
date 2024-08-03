using Microsoft.OData.ModelBuilder;
using System.Reflection;
using CFW.OData.Attributes;
using CFW.OData.Controllers;
using CFW.OData.Handlers;
using CFW.OData.Models;

namespace CFW.OData.Models
{
    internal class ODataRoute
    {
        private static List<Type> _entitySetHandlerTypes = new List<Type>
        {
            { typeof(IODataEntitySetsQueryHandler<,>) },
            { typeof(IODataEntitySetsCustomQueryHandler<,>) },
            { typeof(IODataEntitySetsCreateHandler<,>) },
            { typeof(IODataEntitySetsDeleteHandler<,>) },
            { typeof(IODataEntitySetsBatchHandler<,>) },
            { typeof(IODataEntitySetsBoundActionHandler<,,,>) },
            { typeof(IODataEntitySetsBoundActionHandler<,,>) },
            { typeof(IODataEntitySetsBatchHandler<,>) },
            { typeof(IODataEntitySetsKeyChildBatchHandler<,,,>) },
            { typeof(IODataEntitySetsUnboundActionHandler<,,>) },
            { typeof(IODataEntitySetsUnboundActionHandler<,,,>) },
        };

        private static List<Type> _singletonTypes = new List<Type>
        {
            { typeof(IODataSingletonQueryHandler<>) },
            { typeof(IODataSingletonPatchHandler<>) },
            { typeof(IODataSingletonActionHandler<>) },
        };

        public ODataRoute(string routePrefix, string route)
        {
            if (string.IsNullOrEmpty(route)) throw new Exception("Route is required.");
            RoutePrefix = routePrefix;
            Route = route;
        }

        public List<Type> HandlerTypes { set; get; } = new List<Type>();

        public List<Type> ControllerTypes { private set; get; } = new List<Type>();

        public string RoutePrefix { get; }

        public string Route { get; private set; }

        internal void AddEntry(ODataConventionModelBuilder modelBuilder, string namespacePrefix)
        {
            if (!TryAddEntitySet(modelBuilder, namespacePrefix) && !TryAddSingletonType(modelBuilder))
            {
                throw new InvalidOperationException("Invalid handler types.");
            }
        }

        private bool TryAddEntitySet(ODataConventionModelBuilder modelBuilder, string namespacePrefix)
        {
            var inValidHandlerTypes = HandlerTypes
                .Any(x => x.GetInterfaces().All(i => !i.IsGenericType || !_entitySetHandlerTypes.Contains(i.GetGenericTypeDefinition())));

            if (inValidHandlerTypes)
            {
                return false;
            }

            var handlerInterfaces = HandlerTypes
                .SelectMany(x => x.GetInterfaces().Where(x => x.IsGenericType && _entitySetHandlerTypes.Contains(x.GetGenericTypeDefinition())))
                .ToList();

            var argNames = handlerInterfaces.Select(x => x.GetGenericArguments()[0].FullName + x.GetGenericArguments()[1].FullName)
                .Distinct()
                .ToList();
            if (argNames.Count > 1)
            {
                throw new InvalidOperationException("Invalid handler interface definition.");
            }

            var firstHandlerInterface = handlerInterfaces.First(x => x.GetGenericArguments().Count() == 2);
            var argTypes = firstHandlerInterface
                .GetGenericArguments();
            var odataViewModelType = argTypes[0];

            var entityType = modelBuilder.AddEntityType(odataViewModelType);
            modelBuilder.AddEntitySet(Route, entityType);
            ControllerTypes.Add(typeof(ODataEntitySetsController<,>).MakeGenericType(argTypes));

            var properties = odataViewModelType
                .GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericArguments().Length == 1
                    && (x.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                    || x.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    || x.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                .Where(x => x.PropertyType.GetGenericArguments()[0].Namespace!.StartsWith(namespacePrefix))
                .ToList();
            if (properties.Any())
            {
                foreach (var propertyInfor in properties)
                {
                    var set = modelBuilder.EntitySets.FirstOrDefault(x => x.Name == propertyInfor.Name);
                    if (set is null)
                    {
                        var childSetType = propertyInfor.PropertyType.GetGenericArguments().Single();
                        var keyProp = childSetType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                        if (keyProp is null)
                        {
                            continue;
                        }

                        modelBuilder.AddEntitySet(childSetType.Name, modelBuilder.AddEntityType(childSetType));
                        var propArgTypes = new List<Type>(argTypes.ToList());
                        propArgTypes.Add(childSetType);

                        var idProp = childSetType.GetProperty("Id")!.PropertyType;
                        propArgTypes.Add(idProp);

                        var cType = typeof(ODataEntitySetKeyChildSetsController<,,,>).MakeGenericType(propArgTypes.ToArray());
                        ControllerTypes.Add(cType);

                    }
                }
            }

            var odataActionInfos = HandlerTypes
                .Where(x => !string.IsNullOrWhiteSpace(x.GetCustomAttribute<ODataRoutingAttribute>()?.Action))
                .Select(x => new { HandlerType = x, ActionName = x.GetCustomAttribute<ODataRoutingAttribute>()!.Action })
                .ToList();

            foreach (var actionInfo in odataActionInfos)
            {
                var actionName = actionInfo.ActionName;

                var action = modelBuilder.Action(actionName);
                action.SetBindingParameter(BindingParameterConfiguration.DefaultBindingParameterName, entityType);

                var handlerInterface = actionInfo.HandlerType.GetInterfaces()
                    .Where(x => x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IODataEntitySetsBoundActionHandler<,,,>))
                    .SingleOrDefault();

                if (handlerInterface != null)
                {
                    var args = handlerInterface.GetGenericArguments();
                    ControllerTypes.Add(typeof(ODataEntitySetActionsController<,,,>).MakeGenericType(args));
                }

                handlerInterface = actionInfo.HandlerType.GetInterfaces()
                    .Where(x => x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IODataEntitySetsUnboundActionHandler<,,,>))
                    .SingleOrDefault();

                if (handlerInterface != null)
                {
                    var args = handlerInterface.GetGenericArguments();
                    ControllerTypes.Add(typeof(ODataEntitySetActionsController<,,,>).MakeGenericType(args));
                }

                handlerInterface = actionInfo.HandlerType.GetInterfaces()
                        .Where(x => x.IsGenericType
                            && x.GetGenericTypeDefinition() == typeof(IODataEntitySetsBoundActionHandler<,,>))
                        .SingleOrDefault();
                if (handlerInterface is not null)
                {
                    var args = handlerInterface.GetGenericArguments();
                    ControllerTypes.Add(typeof(ODataEntitySetActionsReturnEmptyController<,,>).MakeGenericType(args));

                    if (args[2] != typeof(EmptyBody))
                        action.Parameter(args[2], "body");
                }

                handlerInterface = actionInfo.HandlerType.GetInterfaces()
                        .Where(x => x.IsGenericType
                            && x.GetGenericTypeDefinition() == typeof(IODataEntitySetsUnboundActionHandler<,,>))
                        .SingleOrDefault();
                if (handlerInterface is not null)
                {
                    var args = handlerInterface.GetGenericArguments();
                    ControllerTypes.Add(typeof(ODataEntitySetActionsReturnEmptyController<,,>).MakeGenericType(args));

                    if (args[2] != typeof(EmptyBody))
                        action.Parameter(args[2], "body");
                }

            }

            return true;
        }

        private bool TryAddSingletonType(ODataConventionModelBuilder modelBuilder)
        {
            var inValidHandlerTypes = HandlerTypes
                .Any(x => x.GetInterfaces().All(i => !i.IsGenericType
                    || !_singletonTypes.Contains(i.GetGenericTypeDefinition())));

            if (inValidHandlerTypes)
            {
                return false;
            }

            var handlerInterfaces = HandlerTypes
                .SelectMany(x => x.GetInterfaces()
                    .Where(x => x.IsGenericType && _singletonTypes.Contains(x.GetGenericTypeDefinition())))
                .ToList();

            if (handlerInterfaces.Any(x => x.GetGenericArguments().Length != 1))
            {
                throw new InvalidOperationException("Invalid singleton handler interface.");
            }

            var firstHandlerInterface = handlerInterfaces.First();
            var odataViewModelType = firstHandlerInterface.GetGenericArguments()[0];
            var singletonType = modelBuilder.AddEntityType(odataViewModelType);
            modelBuilder.AddSingleton(Route, singletonType!);
            ControllerTypes.Add(typeof(ODataSingletonController<>).MakeGenericType(odataViewModelType));

            var actionRoutes = HandlerTypes
                .Select(x => x.GetCustomAttribute<ODataRoutingAttribute>())
                .Where(x => !string.IsNullOrEmpty(x!.Action))
                .Select(x => x!.Action)
                .ToList();

            foreach (var actionRoute in actionRoutes)
            {
                var action = modelBuilder.Action(actionRoute);
                action.SetBindingParameter(BindingParameterConfiguration.DefaultBindingParameterName, singletonType);
            }


            return true;
        }
    }
}
