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
    public class CountryService : ICountryService
    {
        private readonly thandizoContext _context;

        public CountryService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(string countryCode)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.CountryCode.Equals(countryCode));

            var mappedCountry = new AutoMapperHelper<Countries, CountryDTO>().MapToObject(country);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedCountry
            };
        }

        public async Task<OutputResponse> Get()
        {
            var countries = await _context.Countries.OrderBy(x => x.CountryName).ToListAsync();

            var mappedCountries = new AutoMapperHelper<Countries, CountryDTO>().MapToList(countries);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedCountries
            };
        }

        public async Task<OutputResponse> Add(CountryDTO country)
        {
            var isFound = await _context.Countries.AnyAsync(x => x.CountryName.ToLower() == country.CountryName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Country name already exist, duplicates not allowed"
                };
            }

            isFound = await _context.Countries.AnyAsync(x => x.CountryCode.ToLower() == country.CountryCode.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Country code already exist, duplicates not allowed"
                };
            }

            var mappedCountry = new AutoMapperHelper<CountryDTO, Countries>().MapToObject(country);
            mappedCountry.RowAction = "I";
            mappedCountry.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.Countries.AddAsync(mappedCountry);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(CountryDTO country)
        {
            var countryToUpdate = await _context.Countries.FirstOrDefaultAsync(x => x.CountryCode.Equals(country.CountryCode));

            if (countryToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Country specified does not exist, update cancelled"
                };
            }

            //update details
            countryToUpdate.CountryName = country.CountryName;
            countryToUpdate.ExternalReferenceNumber = country.ExternalReferenceNumber;
            countryToUpdate.RowAction = "U";
            countryToUpdate.ModifiedBy = country.CreatedBy;
            countryToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(string countryCode)
        {
            //check if there are any records associated with the specified id
            var isFound = await _context.PatientTravelHistory.AnyAsync(x => x.CountryCode.Equals(countryCode));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified country has patient travel history attached, deletion denied"
                };
            }

            var country = await _context.Countries.FirstOrDefaultAsync(x => x.CountryCode.Equals(countryCode));

            if (country == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Region specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }

        public async Task<OutputResponse> Search(string searchText)
        {
            var countries = await _context.Countries.Where(x => x.CountryName.Contains(searchText)).ToListAsync();

            var mappedCountries = new AutoMapperHelper<Countries, CountryDTO>().MapToList(countries);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedCountries
            };
        }
    }
}
