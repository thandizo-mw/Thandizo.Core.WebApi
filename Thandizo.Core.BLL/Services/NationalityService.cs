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
    public class NationalityService : INationalityService
    {
        private readonly thandizoContext _context;

        public NationalityService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(string nationalityCode)
        {
            var nationality = await _context.Nationalities.FirstOrDefaultAsync(x => x.NationalityCode.Equals(nationalityCode));

            var mappedNationality = new AutoMapperHelper<Nationalities, NationalityDTO>().MapToObject(nationality);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedNationality
            };
        }

        public async Task<OutputResponse> Get()
        {
            var nationalities = await _context.Nationalities.OrderBy(x => x.NationalityName).ToListAsync();

            var mappedNationalities = new AutoMapperHelper<Nationalities, NationalityDTO>().MapToList(nationalities);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedNationalities
            };
        }

        public async Task<OutputResponse> Add(NationalityDTO nationality)
        {
            var isFound = await _context.Nationalities.AnyAsync(x => x.NationalityName.ToLower() == nationality.NationalityName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Nationality name already exist, duplicates not allowed"
                };
            }

            isFound = await _context.Nationalities.AnyAsync(x => x.NationalityCode.ToLower() == nationality.NationalityCode.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Nationality code already exist, duplicates not allowed"
                };
            }

            var mappedNationality = new AutoMapperHelper<NationalityDTO, Nationalities>().MapToObject(nationality);
            mappedNationality.RowAction = "I";
            mappedNationality.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.Nationalities.AddAsync(mappedNationality);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(NationalityDTO nationality)
        {
            var nationalityToUpdate = await _context.Nationalities.FirstOrDefaultAsync(
                x => x.NationalityCode.Equals(nationality.NationalityCode));

            if (nationalityToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Nationality specified does not exist, update cancelled"
                };
            }

            //update details
            nationalityToUpdate.NationalityName = nationality.NationalityName;
            nationalityToUpdate.RowAction = "U";
            nationalityToUpdate.ModifiedBy = nationality.CreatedBy;
            nationalityToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(string nationalityCode)
        {
            //check if there are any records associated with the specified id
            var isFound = await _context.Patients.AnyAsync(x => x.NationalityCode.Equals(nationalityCode));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified nationality has patients attached, deletion denied"
                };
            }

            var nationality = await _context.Nationalities.FirstOrDefaultAsync(x => x.NationalityCode.Equals(nationalityCode));

            if (nationality == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Nationality specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.Nationalities.Remove(nationality);
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
            var nationalities = await _context
                .FromSprocAsync<Nationalities>("search_nationalities", parameters);

            var mappedNationalities = new AutoMapperHelper<Nationalities, NationalityDTO>().MapToList(nationalities);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedNationalities
            };
        }
    }
}
