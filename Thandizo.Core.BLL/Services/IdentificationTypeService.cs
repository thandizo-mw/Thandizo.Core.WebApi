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

        public async Task<OutputResponse> Get(int identificationTypeId)
        {
            var identificationType = await _context.IdentificationTypes.FirstOrDefaultAsync(x => x.IdentificationTypeId.Equals(identificationTypeId));
           
            var mappedIdentificationType = new AutoMapperHelper<IdentificationTypes, IdentificationTypeDTO>().MapToObject(identificationType);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedIdentificationType
            };
        }

        public async Task<OutputResponse> Get()
        {
            var identificationTypes = await _context.IdentificationTypes.OrderBy(x => x.Description).ToListAsync();

            var mappedIdentificationTypes = new AutoMapperHelper<IdentificationTypes, IdentificationTypeDTO>().MapToList(identificationTypes);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedIdentificationTypes
            };
        }

        public async Task<OutputResponse> Add(IdentificationTypeDTO identificationType)
        {
            var isFound = await _context.IdentificationTypes.AnyAsync(x => x.Description.ToLower() == identificationType.Description.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Identification type description already exist, duplicates not allowed"
                };
            }

            var mappedIdentificationType = new AutoMapperHelper<IdentificationTypeDTO, IdentificationTypes>().MapToObject(identificationType);
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

        public async Task<OutputResponse> Update(IdentificationTypeDTO identificationType)
        {
            var identificationTypeToUpdate = await _context.IdentificationTypes.FirstOrDefaultAsync(x => x.IdentificationTypeId.Equals(identificationType.IdentificationTypeId));

            if (identificationTypeToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Identification type specified does not exist, update cancelled"
                };
            }

            //update Identification Type details
            identificationTypeToUpdate.Description = identificationType.Description;
            identificationTypeToUpdate.RowAction = "U";
            identificationTypeToUpdate.ModifiedBy = identificationType.CreatedBy;
            identificationTypeToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int identificationTypeId)
        {
            //check if there are any records associated with the specified Identification Type
            var isFound = await _context.Patients.AnyAsync(x => x.IdentificationTypeId.Equals(identificationTypeId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified identification type has patients attached, deletion denied"
                };
            }
            else
            {
                isFound = await _context.HealthCareWorkers.AnyAsync(x => x.IdentificationTypeId.Equals(identificationTypeId));
                if (isFound)
                {
                    return new OutputResponse
                    {
                        IsErrorOccured = true,
                        Message = "The specified Identification Type has health care workers attached, deletion denied"
                    };
                }
            }


            var identificationType = await _context.IdentificationTypes.FirstOrDefaultAsync(x => x.IdentificationTypeId.Equals(identificationTypeId));

            if (identificationType == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Identification type specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.IdentificationTypes.Remove(identificationType);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
