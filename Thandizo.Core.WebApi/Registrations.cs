using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core.Abstractions;
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
        /// <param name="localStatisticsUrl"></param>
        public static IServiceCollection AddDomainServices(this IServiceCollection services, string localStatisticsUrl)
        {
            services.AddScoped<IStatisticsCacheService>(x => new StatisticsCacheService(localStatisticsUrl, x.GetRequiredService<IRedisDatabase>()));
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<INationalityService, NationalityService>();
            services.AddScoped<IResponseTeamMappingService, ResponseTeamMappingService>();
            services.AddScoped<IResponseTeamMemberService, ResponseTeamMemberService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<IDataCenterService, DataCenterService>();
            services.AddScoped<IFacilityTypeService, FacilityTypeService>();
            services.AddScoped<IHealthCareWorkerService, HealthCareWorkerService>();
            return services.AddScoped<IIdentificationTypeService, IdentificationTypeService>();
        }
    }
}
