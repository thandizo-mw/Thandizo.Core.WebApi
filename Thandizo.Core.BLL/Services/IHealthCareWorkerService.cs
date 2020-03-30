using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IHealthCareWorkerService
    {
        Task<OutputResponse> Add(HealthCareWorkerDTO healthCareWorker);
        Task<OutputResponse> Delete(int workerId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int workerId);
        Task<OutputResponse> Update(HealthCareWorkerDTO healthCareWorker);
    }
}