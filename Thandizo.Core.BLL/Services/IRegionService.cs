using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IRegionService
    {
        Task<OutputResponse> Add(RegionDTO region);
        Task<OutputResponse> Delete(int regionId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int regionId);
        Task<OutputResponse> Update(RegionDTO region);
    }
}