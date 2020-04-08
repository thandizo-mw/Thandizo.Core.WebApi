using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.EF.Extensions;
using Thandizo.DAL.Models;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly thandizoContext _context;

        public DistrictService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(string districtCode)
        {
            var District = await _context.Districts.FirstOrDefaultAsync(x => x.DistrictCode.Equals(districtCode));
           
            var mappedDistrict = new AutoMapperHelper<Districts, DistrictDTO>().MapToObject(District);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedDistrict
            };
        }

        public async Task<OutputResponse> Get()
        {
            var Districts = await _context.Districts.OrderBy(x => x.DistrictName).ToListAsync();

            var mappedDistricts = new AutoMapperHelper<Districts, DistrictDTO>().MapToList(Districts);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedDistricts
            };
        }

        public async Task<OutputResponse> Add(DistrictDTO District)
        {
            var isFound = await _context.Districts.AnyAsync(x => x.DistrictName.ToLower() == District.DistrictName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "District name already exist, duplicates not allowed"
                };
            }

            var mappedDistrict = new AutoMapperHelper<DistrictDTO, Districts>().MapToObject(District);
            mappedDistrict.RowAction = "I";
            mappedDistrict.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.Districts.AddAsync(mappedDistrict);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(DistrictDTO district)
        {
            var districtToUpdate = await _context.Districts.FirstOrDefaultAsync(x => x.DistrictCode.Equals(district.DistrictCode));

            if (districtToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "District specified does not exist, update cancelled"
                };
            }

            //update District details
            districtToUpdate.DistrictName = district.DistrictName;
            districtToUpdate.RegionId = district.RegionId;
            districtToUpdate.Longitude = district.Longitude;
            districtToUpdate.Latitude = district.Latitude;
            districtToUpdate.RowAction = "U";
            districtToUpdate.ModifiedBy = district.CreatedBy;
            districtToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(string districtCode)
        {
            //check if there are any records associated with the specified District
            var isFound = await _context.Patients.AnyAsync(x => x.DistrictCode.Equals(districtCode));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified district has patients attached, deletion denied"
                };
            }
            else
            {
                isFound = await _context.DataCenters.AnyAsync(x => x.DistrictCode.Equals(districtCode));
                if (isFound)
                {
                    return new OutputResponse
                    {
                        IsErrorOccured = true,
                        Message = "The specified district has data centers attached, deletion denied"
                    };
                }
                else
                {
                    isFound = await _context.PatientLocationMovements.AnyAsync(x => x.DistrictCode.Equals(districtCode));
                    if (isFound)
                    {
                        return new OutputResponse
                        {
                            IsErrorOccured = true,
                            Message = "The specified district has patient location movements attached, deletion denied"
                        };
                    }
                }
            }


            var District = await _context.Districts.FirstOrDefaultAsync(x => x.DistrictCode.Equals(districtCode));

            if (District == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "District specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.Districts.Remove(District);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }

        public async Task<OutputResponse> Search(string searchText)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_search_text", searchText }
            };
            var districts = await _context
                .FromSprocAsync<Districts>("search_districts", parameters);

            var mappedDistricts = new AutoMapperHelper<Districts, DistrictDTO>().MapToList(districts);
            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedDistricts
            };
        }
    }
}
