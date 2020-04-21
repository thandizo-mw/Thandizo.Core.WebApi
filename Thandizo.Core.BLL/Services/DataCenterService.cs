using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.Core;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.DataCenters.Responses;
using Thandizo.DataModels.General;

namespace Thandizo.Core.BLL.Services
{
    public class DataCenterService : IDataCenterService
    {
        private readonly thandizoContext _context;

        public DataCenterService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int centerId)
        {
            var dataCenter = await _context.DataCenters.Where(x => x.CenterId.Equals(centerId))
                .Select(x => new DataCenterResponse 
                {
                    CenterId = x.CenterId,
                    CenterName = x.CenterName,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    FacilityTypeId = x.FacilityTypeId,
                    FacilityTypeName = x.FacilityType.FacilityTypeName,
                    IsHealthFacility = x.IsHealthFacility,
                    ModifiedBy = x.ModifiedBy,
                    RowAction = x.RowAction
                }).FirstOrDefaultAsync();

           
            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = dataCenter
            };
        }

        public async Task<OutputResponse> Get()
        {
            var dataCenters = await _context.DataCenters.OrderBy(x => x.CenterName)
                .Select(x => new DataCenterResponse
                {
                    CenterId = x.CenterId,
                    CenterName = x.CenterName,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictCodeNavigation.DistrictName,
                    FacilityTypeId = x.FacilityTypeId,
                    FacilityTypeName = x.FacilityType.FacilityTypeName,
                    IsHealthFacility = x.IsHealthFacility,
                    ModifiedBy = x.ModifiedBy,
                    RowAction = x.RowAction
                }).ToListAsync();


            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = dataCenters
            };
        }

        public async Task<OutputResponse> Add(DataCenterDTO dataCenter)
        {
            var isFound = await _context.DataCenters.AnyAsync(x => x.CenterName.ToLower() == dataCenter.CenterName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Data center name already exist, duplicates not allowed"
                };
            }

            var mappedDataCenter = new AutoMapperHelper<DataCenterDTO, DataCenters>().MapToObject(dataCenter);
            mappedDataCenter.RowAction = "I";
            mappedDataCenter.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.DataCenters.AddAsync(mappedDataCenter);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(DataCenterDTO dataCenter)
        {
            var dataCenterToUpdate = await _context.DataCenters.FirstOrDefaultAsync(x => x.CenterId.Equals(dataCenter.CenterId));

            if (dataCenterToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Data center specified does not exist, update cancelled"
                };
            }

            //update data center details
            dataCenterToUpdate.CenterName = dataCenter.CenterName;
            dataCenterToUpdate.DistrictCode = dataCenter.DistrictCode;
            dataCenterToUpdate.FacilityTypeId = dataCenter.FacilityTypeId;
            dataCenterToUpdate.IsHealthFacility = dataCenter.IsHealthFacility;
            dataCenterToUpdate.RowAction = "U";
            dataCenterToUpdate.ModifiedBy = dataCenter.CreatedBy;
            dataCenterToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int centerId)
        {
            //check if there are any records associated with the specified Data center
            var isFound = await _context.Patients.AnyAsync(x => x.DistrictCode.Equals(centerId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified data center has patients attached, deletion denied"
                };
            }
            else
            {
                isFound = await _context.HealthCareWorkers.AnyAsync(x => x.DataCenterId.Equals(centerId));
                if (isFound)
                {
                    return new OutputResponse
                    {
                        IsErrorOccured = true,
                        Message = "The specified data center has health care workers centers attached, deletion denied"
                    };
                }
                else
                {
                    isFound = await _context.HealthCareWorkers.AnyAsync(x => x.DataCenterId.Equals(centerId));
                    if (isFound)
                    {
                        return new OutputResponse
                        {
                            IsErrorOccured = true,
                            Message = "The specified data center has health care workers attached, deletion denied"
                        };
                    }
                    else
                    {
                        if ((await _context.PatientFacilityMovements.AnyAsync(x => x.FromDataCenterId.Equals(centerId))) || (await _context.PatientFacilityMovements.AnyAsync(x => x.ToDataCenterId.Equals(centerId))))
                        {
                            return new OutputResponse
                            {
                                IsErrorOccured = true,
                                Message = "The specified data center has patient facility movements attached, deletion denied"
                            };
                        }
                    }
                }
            }


            var DataCenter = await _context.DataCenters.FirstOrDefaultAsync(x => x.CenterId.Equals(centerId));

            if (DataCenter == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Data center specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.DataCenters.Remove(DataCenter);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
