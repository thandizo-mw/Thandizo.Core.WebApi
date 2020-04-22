using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public class ResponseTeamMemberService : IResponseTeamMemberService
    {
        private readonly thandizoContext _context;

        public ResponseTeamMemberService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int teamMemberId)
        {
            var teamMember = await _context.ResponseTeamMembers.FirstOrDefaultAsync(x => x.TeamMemberId.Equals(teamMemberId));

            var mappedTeamMember = new AutoMapperHelper<ResponseTeamMembers, ResponseTeamMemberDTO>().MapToObject(teamMember);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedTeamMember
            };
        }

        public async Task<OutputResponse> Get()
        {
            var teamMembers = await _context.ResponseTeamMembers.OrderBy(x => x.Surname).ToListAsync();

            var mappedTeamMembers = new AutoMapperHelper<ResponseTeamMembers, ResponseTeamMemberDTO>().MapToList(teamMembers);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedTeamMembers
            };
        }

        public async Task<OutputResponse> Add(ResponseTeamMemberDTO teamMember)
        {
            var isFound = await _context.ResponseTeamMembers.AnyAsync(x => x.PhoneNumber == teamMember.PhoneNumber);
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Team member with this phone number already exist, duplicates not allowed"
                };
            }

            var mappedTeamMember = new AutoMapperHelper<ResponseTeamMemberDTO, ResponseTeamMembers>().MapToObject(teamMember);
            mappedTeamMember.RowAction = "I";
            mappedTeamMember.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.ResponseTeamMembers.AddAsync(mappedTeamMember);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(ResponseTeamMemberDTO teamMember)
        {
            var teamMemberToUpdate = await _context.ResponseTeamMembers.FirstOrDefaultAsync(x => x.TeamMemberId.Equals(teamMember.TeamMemberId));

            if (teamMemberToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Team member specified does not exist, update cancelled"
                };
            }

            //update details
            teamMemberToUpdate.FirstName = teamMember.FirstName;
            teamMemberToUpdate.OtherNames = teamMember.OtherNames;
            teamMemberToUpdate.Surname = teamMember.Surname;
            teamMemberToUpdate.EmailAddress = teamMember.EmailAddress;
            teamMemberToUpdate.PhoneNumber = teamMember.PhoneNumber;
            teamMemberToUpdate.RowAction = "U";
            teamMemberToUpdate.ModifiedBy = teamMember.CreatedBy;
            teamMemberToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int teamMemberId)
        {
            var teamMember = await _context.ResponseTeamMembers.FirstOrDefaultAsync(x => x.TeamMemberId.Equals(teamMemberId));

            if (teamMember == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Team member specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.ResponseTeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
