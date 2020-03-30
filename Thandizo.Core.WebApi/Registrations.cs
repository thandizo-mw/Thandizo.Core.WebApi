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
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IDataCenterService, DataCenterService>();
            services.AddScoped<IFacilityTypeService, FacilityTypeService>();
            services.AddScoped<IHealthCareWorkerService, HealthCareWorkerService>();
            return services.AddScoped<IIdentificationTypeService, IdentificationTypeService>();
        }
    }
}
