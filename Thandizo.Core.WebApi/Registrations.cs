using Microsoft.Extensions.DependencyInjection;
using Thandizo.Core.BLL.Services;

namespace Thandizo.Core.WebApi
{
    public static class Registrations
    {
        /// <summary>
        /// Registers domain services to the specified
        /// service descriptor
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<INationalityService, NationalityService>();
            return services.AddScoped<IRegionService, RegionService>();
        }
    }
}
