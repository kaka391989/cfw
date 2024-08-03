using CFW.Core.Services.Validations;
using Microsoft.Extensions.DependencyInjection;

namespace CFW.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ValidationService>();

            return services;
        }
    }
}
