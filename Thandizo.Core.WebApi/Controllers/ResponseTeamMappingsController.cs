using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.Filters;
using Thandizo.ApiExtensions.General;
using Thandizo.Core.BLL.Services;
using Thandizo.DataModels.Core;

namespace Thandizo.Core.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseTeamMappingsController : ControllerBase
    {
        IResponseTeamMappingService _service;

        public ResponseTeamMappingsController(IResponseTeamMappingService service)
        {
            _service = service;
        }

        [HttpGet("GetById")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetById([FromQuery] int mappingId)
        {
            var response = await _service.Get(mappingId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetByMember")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetByMember([FromQuery]int teamMemberId)
        {
            var response = await _service.GetByMember(teamMemberId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpPost("Add")]
        [ValidateModelState]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]ResponseTeamMappingDTO mapping)
        {
            var outputHandler = await _service.Add(mapping);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Created("", outputHandler.Message);
        }

        [HttpPut("Update")]
        [ValidateModelState]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]ResponseTeamMappingDTO mapping)
        {
            var outputHandler = await _service.Update(mapping);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }

        [HttpDelete("Delete")]
        [CatchException(MessageHelper.DeleteError)]
        public async Task<IActionResult> Delete([FromQuery]int mappingId)
        {
            var outputHandler = await _service.Delete(mappingId);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }

        [HttpGet("GetMemberEmailAddress")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetMemberEmailAddress([FromQuery]string districtCode)
        {
            var response = await _service.GetMemberEmailAddress(districtCode);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetMemberPhoneNumbers")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetMemberPhoneNumbers([FromQuery]string districtCode)
        {
            var response = await _service.GetMemberPhoneNumbers(districtCode);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }
    }
}