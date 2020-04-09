using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IResponseTeamMappingService
    {
        Task<OutputResponse> Add(ResponseTeamMappingDTO teamMapping);
        Task<OutputResponse> Delete(int mappingId);
        Task<OutputResponse> Get(int mappingId);
        Task<OutputResponse> GetByMember(int teamMemberId);
        Task<OutputResponse> GetMemberEmailAddress(string districtCode);
        Task<OutputResponse> GetMemberPhoneNumbers(string districtCode);
        Task<OutputResponse> Update(ResponseTeamMappingDTO teamMapping);
    }
}