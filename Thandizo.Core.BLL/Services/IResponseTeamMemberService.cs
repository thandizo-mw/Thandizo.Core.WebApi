using System.Threading.Tasks;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public interface IResponseTeamMemberService
    {
        Task<OutputResponse> Add(ResponseTeamMemberDTO teamMember);
        Task<OutputResponse> Delete(int teamMemberId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int teamMemberId);
        Task<OutputResponse> Update(ResponseTeamMemberDTO teamMember);
    }
}