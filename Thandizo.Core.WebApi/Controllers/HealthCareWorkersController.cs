﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.Filters;
using Thandizo.ApiExtensions.General;
using Thandizo.Core.BLL.Services;
using Thandizo.DataModels.DataCenters;

namespace Thandizo.Core.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCareWorkersController : ControllerBase
    {
        IHealthCareWorkerService _service;

        public HealthCareWorkersController(IHealthCareWorkerService service)
        {
            _service = service;
        }

        [HttpGet("GetById")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetById([FromQuery] int workerId)
        {
            var response = await _service.Get(workerId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetByDataCenter")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetByDataCenter([FromQuery] int centerId)
        {
            var response = await _service.GetByDataCenter(centerId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetAll")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetAll()
        {            
            var response = await _service.Get();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpPost("Add")]
        [ValidateModelState]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]HealthCareWorkerDTO healthCareWorker)
        {
            var outputHandler = await _service.Add(healthCareWorker);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Created("", outputHandler.Message);
        }

        [HttpPut("Update")]
        [ValidateModelState]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]HealthCareWorkerDTO healthCareWorker)
        {
            var outputHandler = await _service.Update(healthCareWorker);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }

        [HttpDelete("Delete")]
        [CatchException(MessageHelper.DeleteError)]
        public async Task<IActionResult> Delete([FromQuery]int workerId)
        {
            var outputHandler = await _service.Delete(workerId);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }
    }
}