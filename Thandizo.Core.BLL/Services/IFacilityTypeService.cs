using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IFacilityTypeService
    {
        Task<OutputResponse> Add(FacilityTypeDTO facilityType);
        Task<OutputResponse> Delete(int facilityTypeId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int facilityTypeId);
        Task<OutputResponse> Update(FacilityTypeDTO facilityType);
    }
}