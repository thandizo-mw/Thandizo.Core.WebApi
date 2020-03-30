using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public class FacilityTypeService : IFacilityTypeService
    {
        private readonly thandizoContext _context;

        public FacilityTypeService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int facilityTypeId)
        {
            var FacilityType = await _context.FacilityTypes.FirstOrDefaultAsync(x => x.FacilityTypeId.Equals(facilityTypeId));
           
            var mappedFacilityType = new AutoMapperHelper<FacilityTypes, FacilityTypeDTO>().MapToObject(FacilityType);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedFacilityType
            };
        }

        public async Task<OutputResponse> Get()
        {
            var facilityTypes = await _context.FacilityTypes.OrderBy(x => x.FacilityTypeName).ToListAsync();

            var mappedFacilityTypes = new AutoMapperHelper<FacilityTypes, FacilityTypeDTO>().MapToList(facilityTypes);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedFacilityTypes
            };
        }

        public async Task<OutputResponse> Add(FacilityTypeDTO facilityType)
        {
            var isFound = await _context.FacilityTypes.AnyAsync(x => x.FacilityTypeName.ToLower() == facilityType.FacilityTypeName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Identification type description already exist, duplicates not allowed"
                };
            }

            var mappedFacilityType = new AutoMapperHelper<FacilityTypeDTO, FacilityTypes>().MapToObject(facilityType);
            mappedFacilityType.RowAction = "I";
            mappedFacilityType.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.FacilityTypes.AddAsync(mappedFacilityType);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(FacilityTypeDTO facilityType)
        {
            var facilityTypeToUpdate = await _context.FacilityTypes.FirstOrDefaultAsync(x => x.FacilityTypeId.Equals(facilityType.FacilityTypeId));

            if (facilityTypeToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Identification type specified does not exist, update cancelled"
                };
            }

            //update Identification Type details
            facilityTypeToUpdate.FacilityTypeName = facilityType.FacilityTypeName;
            facilityTypeToUpdate.RowAction = "U";
            facilityTypeToUpdate.ModifiedBy = facilityType.CreatedBy;
            facilityTypeToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int facilityTypeId)
        {
            //check if there are any records associated with the specified Identification Type
            var isFound = await _context.DataCenters.AnyAsync(x => x.FacilityTypeId.Equals(facilityTypeId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified identification type has data centers attached, deletion denied"
                };
            }


            var FacilityType = await _context.FacilityTypes.FirstOrDefaultAsync(x => x.FacilityTypeId.Equals(facilityTypeId));

            if (FacilityType == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Facility type specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.FacilityTypes.Remove(FacilityType);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
