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
    public class RegionService : IRegionService
    {
        private readonly thandizoContext _context;

        public RegionService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int regionId)
        {
            var region = await _context.Regions.FirstOrDefaultAsync(x => x.RegionId.Equals(regionId));
           
            var mappedRegion = new AutoMapperHelper<Regions, RegionDTO>().MapToObject(region);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedRegion
            };
        }

        public async Task<OutputResponse> Get()
        {
            var regions = await _context.Regions.OrderBy(x => x.RegionName).ToListAsync();

            var mappedRegions = new AutoMapperHelper<Regions, RegionDTO>().MapToList(regions);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedRegions
            };
        }

        public async Task<OutputResponse> Add(RegionDTO region)
        {
            var isFound = await _context.Regions.AnyAsync(x => x.RegionName.ToLower() == region.RegionName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Region name already exist, duplicates not allowed"
                };
            }

            var mappedRegion = new AutoMapperHelper<RegionDTO, Regions>().MapToObject(region);
            mappedRegion.RowAction = "I";
            mappedRegion.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.Regions.AddAsync(mappedRegion);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(RegionDTO region)
        {
            var regionToUpdate = await _context.Regions.FirstOrDefaultAsync(x => x.RegionId.Equals(region.RegionId));

            if (regionToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Region specified does not exist, update cancelled"
                };
            }

            //update region details
            regionToUpdate.RegionName = region.RegionName;
            regionToUpdate.RowAction = "U";
            regionToUpdate.ModifiedBy = region.CreatedBy;
            regionToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int regionId)
        {
            //check if there are any records associated with the specified region
            var isFound = await _context.Districts.AnyAsync(x => x.RegionId.Equals(regionId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified region has districts attached, deletion denied"
                };
            }

            var region = await _context.Regions.FirstOrDefaultAsync(x => x.RegionId.Equals(regionId));

            if (region == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Region specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
