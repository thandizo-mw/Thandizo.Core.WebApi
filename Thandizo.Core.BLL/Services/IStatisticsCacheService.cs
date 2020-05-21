using System.Threading.Tasks;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IStatisticsCacheService
    {
        Task<OutputResponse> GetNationalStatistics();
    }
}
