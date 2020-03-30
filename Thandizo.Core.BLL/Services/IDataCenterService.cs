using System.Threading.Tasks;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IDataCenterService
    {
        Task<OutputResponse> Add(DataCenterDTO dataCenter);
        Task<OutputResponse> Delete(int centerId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int centerId);
        Task<OutputResponse> Update(DataCenterDTO dataCenter);
    }
}