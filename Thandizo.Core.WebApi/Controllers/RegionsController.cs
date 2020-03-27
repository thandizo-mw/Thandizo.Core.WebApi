using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.Filters;
using Thandizo.Core.BLL.Services;

namespace Thandizo.Core.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        IRegionService _service;

        public RegionsController(IRegionService service)
        {
            _service = service;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int regionId)
        {
            var response = await _service.Get(regionId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Result);
        }

        [HttpGet("GetAll")]
        [CatchException("testing")]
        public async Task<IActionResult> GetAll()
        {            
            var response = await _service.Get();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Result);
        }
    }
}