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
    public class IdentificationTypeService : IIdentificationTypeService
    {
        private readonly thandizoContext _context;

        public IdentificationTypeService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int IdentificationTypeId)
        {
            var IdentificationType = await _context.IdentificationTypes.FirstOrDefaultAsync(x => x.IdentificationTypeId.Equals(IdentificationTypeId));
           
            var mappedIdentificationType = new AutoMapperHelper<IdentificationTypes, IdentificationTypeDTO>().MapToObject(IdentificationType);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedIdentificationType
            };
        }

        public async Task<OutputResponse> Get()
        {
            var IdentificationTypes = await _context.IdentificationTypes.OrderBy(x => x.Description).ToListAsync();

            var mappedIdentificationTypes = new AutoMapperHelper<IdentificationTypes, IdentificationTypeDTO>().MapToList(IdentificationTypes);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedIdentificationTypes
            };
        }

        public async Task<OutputResponse> Add(IdentificationTypeDTO IdentificationType)
        {
            var isFound = await _context.IdentificationTypes.AnyAsync(x => x.Description.ToLower() == IdentificationType.Description.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Identification Type description already exist, duplicates not allowed"
                };
            }

            var mappedIdentificationType = new AutoMapperHelper<IdentificationTypeDTO, IdentificationTypes>().MapToObject(IdentificationType);
            mappedIdentificationType.RowAction = "I";
            mappedIdentificationType.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.IdentificationTypes.AddAsync(mappedIdentificationType);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(IdentificationTypeDTO IdentificationType)
        {
            var IdentificationTypeToUpdate = await _context.IdentificationTypes.FirstOrDefaultAsync(x => x.IdentificationTypeId.Equals(IdentificationType.IdentificationTypeId));

            if (IdentificationTypeToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Identification Type specified does not exist, update cancelled"
                };
            }

            //update Identification Type details
            IdentificationTypeToUpdate.Description = IdentificationType.Description;
            IdentificationTypeToUpdate.RowAction = "U";
            IdentificationTypeToUpdate.ModifiedBy = IdentificationType.CreatedBy;
            IdentificationTypeToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int IdentificationTypeId)
        {
            //check if there are any records associated with the specified Identification Type
            var isFound = await _context.Patients.AnyAsync(x => x.IdentificationTypeId.Equals(IdentificationTypeId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified Identification Type has patients attached, deletion denied"
                };
            }
            else
            {
                isFound = await _context.HealthCareWorkers.AnyAsync(x => x.IdentificationTypeId.Equals(IdentificationTypeId));
                if (isFound)
                {
                    return new OutputResponse
                    {
                        IsErrorOccured = true,
                        Message = "The specified Identification Type has health care workers attached, deletion denied"
                    };
                }
            }


            var IdentificationType = await _context.IdentificationTypes.FirstOrDefaultAsync(x => x.IdentificationTypeId.Equals(IdentificationTypeId));

            if (IdentificationType == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "IdentificationType specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.IdentificationTypes.Remove(IdentificationType);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
