using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IDistrictService
    {
        Task<OutputResponse> Add(DistrictDTO district);
        Task<OutputResponse> Delete(string districtCode);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(string districtCode);
        Task<OutputResponse> Update(DistrictDTO district);
    }
}