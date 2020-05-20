using AngleDimension.Standard.Http.HttpServices;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.ExternalSystems;
using System.Net.Http;

namespace Thandizo.Core.BLL.Services
{
    public class StatisticsCacheService : IStatisticsCacheService
    {
        private readonly string _baseUrl;
        private readonly IRedisDatabase _database;

        public StatisticsCacheService(string baseUrl, IRedisDatabase database)
        {
            _baseUrl = baseUrl;
            _database = database;
        }

        private async Task<DhisNationalStatisticsDTO> FromCache()
        {
            var statistics = await _database.GetAsync<DhisNationalStatisticsDTO>("national-stats");
            return statistics;
        }

        public async Task<OutputResponse> GetNationalStatistics()
        {
            var result = new OutputResponse { IsErrorOccured = false };
            var statistics = default(DhisNationalStatisticsDTO);
            try
            {
                var endpoint = string.Concat(_baseUrl, "aggregates");
                var response = await HttpRequestFactory.Get(endpoint, TimeSpan.FromMilliseconds(1500));

                if (response.IsSuccessStatusCode)
                {
                    statistics = response.ContentAsType<DhisNationalStatisticsDTO>();
                    // store in cache
                    await _database.AddAsync("national-stats", statistics, flag: CommandFlags.FireAndForget);
                }
                else
                {
                    if (await _database.ExistsAsync("national-stats"))
                    {
                        statistics = await FromCache();
                    }
                    else
                    {
                        result.IsErrorOccured = true;
                    }
                }
            }
            catch(Exception)
            {
                statistics = await FromCache();
            }
            finally
            {
                result.Result = statistics;
            }
            return result;
        }
    }
}
