using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface INationalityService
    {
        Task<OutputResponse> Add(NationalityDTO nationality);
        Task<OutputResponse> Delete(string nationalityCode);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(string nationalityCode);
        Task<OutputResponse> Update(NationalityDTO nationality);
    }
}