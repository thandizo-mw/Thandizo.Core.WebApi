using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IIdentificationTypeService
    {
        Task<OutputResponse> Add(IdentificationTypeDTO identificationType);
        Task<OutputResponse> Delete(int identificationTypeId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int identificationTypeId);
        Task<OutputResponse> Update(IdentificationTypeDTO identificationType);
    }
}