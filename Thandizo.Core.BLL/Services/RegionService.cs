using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
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

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = region
            };
        }

        public async Task<OutputResponse> Get()
        {
            var regions = await _context.Regions.OrderBy(x => x.RegionName).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = regions
            };
        }

        public async Task<OutputResponse> Add(RegionDTO region)
        {
            var regions = await _context.Regions.OrderBy(x => x.RegionName).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = "The record has been created successfully"
            };
        }

        public async Task<OutputResponse> Update(RegionDTO region)
        {
            var regions = await _context.Regions.OrderBy(x => x.RegionName).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = "The record has been updated successfully"
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
                    Message = "Region specified does not exist"
                };
            }

            //deletes the record permanently
            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = "The record has been deleted successfully"
            };
        }
    }
}
