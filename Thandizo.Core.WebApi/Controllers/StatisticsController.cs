using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Thandizo.Core.BLL.Services;

namespace Thandizo.Core.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsCacheService _service;

        public StatisticsController(IStatisticsCacheService service)
        {
            _service = service;
        }


        [HttpGet("NationalStatistics")]
        public async Task<IActionResult> GetNationalStatistics()
        {
            var response = await _service.GetNationalStatistics();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Result);
        }
    }
}