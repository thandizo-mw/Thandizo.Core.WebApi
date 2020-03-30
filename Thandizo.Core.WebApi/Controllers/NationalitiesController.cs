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
    public class NationalitiesController : ControllerBase
    {
        INationalityService _service;

        public NationalitiesController(INationalityService service)
        {
            _service = service;
        }

        [HttpGet("GetByCode")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetByCode([FromQuery] string nationalityCode)
        {
            var response = await _service.Get(nationalityCode);

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
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]NationalityDTO nationality)
        {
            var outputHandler = await _service.Add(nationality);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Created("", outputHandler);
        }

        [HttpPut("Update")]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]NationalityDTO nationality)
        {
            var outputHandler = await _service.Update(nationality);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Ok(outputHandler);
        }

        [HttpDelete("Delete")]
        [CatchException(MessageHelper.DeleteError)]
        public async Task<IActionResult> Delete([FromQuery]string nationalityCode)
        {
            var outputHandler = await _service.Delete(nationalityCode);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Ok(outputHandler);
        }
    }
}