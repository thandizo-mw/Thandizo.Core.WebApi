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
    public class HealthCareWorkerService : IHealthCareWorkerService
    {
        private readonly thandizoContext _context;

        public HealthCareWorkerService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int workerId)
        {
            var healthCareWorker = await _context.HealthCareWorkers.FirstOrDefaultAsync(x => x.WorkerId.Equals(workerId));
           
            var mappedHealthCareWorker = new AutoMapperHelper<HealthCareWorkers, HealthCareWorkerDTO>().MapToObject(healthCareWorker);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedHealthCareWorker
            };
        }

        public async Task<OutputResponse> Get()
        {
            var healthCareWorkers = await _context.HealthCareWorkers.OrderBy(x => x.LastName).ToListAsync();

            var mappedHealthCareWorkers = new AutoMapperHelper<HealthCareWorkers, HealthCareWorkerDTO>().MapToList(healthCareWorkers);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedHealthCareWorkers
            };
        }

        public async Task<OutputResponse> Add(HealthCareWorkerDTO healthCareWorker)
        {
            var isFound = await _context.HealthCareWorkers.AnyAsync(x => x.IdentificationNumber.ToLower() == healthCareWorker.IdentificationNumber.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Health care worker identification number already exist, duplicates not allowed"
                };
            }

            var mappedHealthCareWorker = new AutoMapperHelper<HealthCareWorkerDTO, HealthCareWorkers>().MapToObject(healthCareWorker);
            mappedHealthCareWorker.RowAction = "I";
            mappedHealthCareWorker.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.HealthCareWorkers.AddAsync(mappedHealthCareWorker);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(HealthCareWorkerDTO healthCareWorker)
        {
            var healthCareWorkerToUpdate = await _context.HealthCareWorkers.FirstOrDefaultAsync(x => x.WorkerId.Equals(healthCareWorker.WorkerId));

            if (healthCareWorkerToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Health care worker specified does not exist, update cancelled"
                };
            }

            //update Identification Type details
            healthCareWorkerToUpdate.FirstName = healthCareWorker.FirstName;
            healthCareWorkerToUpdate.OtherNames = healthCareWorker.OtherNames;
            healthCareWorkerToUpdate.LastName = healthCareWorker.LastName;
            healthCareWorkerToUpdate.DataCenterId = healthCareWorker.DataCenterId;
            healthCareWorkerToUpdate.DateOfBirth = healthCareWorker.DateOfBirth;
            healthCareWorkerToUpdate.EmailAddress = healthCareWorker.EmailAddress;
            healthCareWorkerToUpdate.Gender = healthCareWorker.Gender;
            healthCareWorkerToUpdate.IdentificationNumber = healthCareWorker.IdentificationNumber;
            healthCareWorkerToUpdate.IdentificationTypeId = healthCareWorker.IdentificationTypeId;
            healthCareWorkerToUpdate.PhoneNumber = healthCareWorker.PhoneNumber;
            healthCareWorkerToUpdate.RowAction = "U";
            healthCareWorkerToUpdate.ModifiedBy = healthCareWorker.CreatedBy;
            healthCareWorkerToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int workerId)
        {
            var healthCareWorker = await _context.HealthCareWorkers.FirstOrDefaultAsync(x => x.WorkerId.Equals(workerId));

            if (healthCareWorker == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Health care worker specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.HealthCareWorkers.Remove(healthCareWorker);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}
