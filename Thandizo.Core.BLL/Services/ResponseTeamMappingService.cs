using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.Core.Responses;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public class ResponseTeamMappingService : IResponseTeamMappingService
    {
        private readonly thandizoContext _context;

        public ResponseTeamMappingService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int mappingId)
        {
            var teamMapping = await _context.ResponseTeamMappings.Where(x => x.MappingId.Equals(mappingId))
                .Select(x => new TeamMappingResponse
                {
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    MappingId = x.MappingId,
                    TeamMemberId = x.TeamMemberId,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    DistrictCode = x.DistrictCode,
                    ModifiedBy = x.ModifiedBy,
                    RowAction = x.RowAction
                }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = teamMapping
            };
        }

        public async Task<OutputResponse> GetByMember(int teamMemberId)
        {
            var teamMappings = await _context.ResponseTeamMappings.Where(x => x.TeamMemberId.Equals(teamMemberId))
                .Select(x => new TeamMappingResponse
                {
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    MappingId = x.MappingId,
                    TeamMemberId = x.TeamMemberId,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    DistrictCode = x.DistrictCode,
                    ModifiedBy = x.ModifiedBy,
                    RowAction = x.RowAction
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = teamMappings
            };
        }

        public async Task<OutputResponse> Add(ResponseTeamMappingDTO teamMapping)
        {
            var isFound = await _context.ResponseTeamMappings.AnyAsync(x => x.TeamMemberId == teamMapping.TeamMemberId
                && x.DistrictCode == teamMapping.DistrictCode);
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Team member with this district already exist, duplicates not allowed"
                };
            }

            var mappedTeamMapping = new AutoMapperHelper<ResponseTeamMappingDTO, ResponseTeamMappings>().MapToObject(teamMapping);
            mappedTeamMapping.RowAction = "I";
            mappedTeamMapping.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.ResponseTeamMappings.AddAsync(mappedTeamMapping);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(ResponseTeamMappingDTO teamMapping)
        {
            var teamMappingToUpdate = await _context.ResponseTeamMappings.FirstOrDefaultAsync(x => x.MappingId.Equals(teamMapping.MappingId));

            if (teamMappingToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Team member mapping specified does not exist, update cancelled"
                };
            }

            //update details
            teamMappingToUpdate.TeamMemberId = teamMapping.TeamMemberId;
            teamMappingToUpdate.DistrictCode = teamMapping.DistrictCode;
            teamMappingToUpdate.RowAction = "U";
            teamMappingToUpdate.ModifiedBy = teamMapping.CreatedBy;
            teamMappingToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int mappingId)
        {
            var teamMapping = await _context.ResponseTeamMappings.FirstOrDefaultAsync(x => x.MappingId.Equals(mappingId));

            if (teamMapping == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Team member mapping specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.ResponseTeamMappings.Remove(teamMapping);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }

        public async Task<OutputResponse> GetMemberPhoneNumbers(string districtCode)
        {
            var phoneNumbers = await _context.ResponseTeamMappings.Where(x => x.DistrictCode.Equals(districtCode))
                .Select(x => x.TeamMember.PhoneNumber).ToArrayAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = phoneNumbers
            };
        }

        public async Task<OutputResponse> GetMemberEmailAddress(string districtCode)
        {
            var emailAddresses = await _context.ResponseTeamMappings.Where(x => x.DistrictCode.Equals(districtCode)
                && (x.TeamMember.EmailAddress != null || x.TeamMember.EmailAddress != ""))
                .Select(x => x.TeamMember.EmailAddress).ToArrayAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = emailAddresses
            };
        }
    }
}
