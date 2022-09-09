using headlessCMS.Mappers;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("draft")]
    public class DraftApiController : Controller
    {
        private readonly ApiService _apiService;

        public DraftApiController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost("getdata")]
        public async Task<IActionResult> GetDataAsync(SelectQueryParametersDataCollection selectQueryParametersDataCollection)
        {
            DraftApiRequestMapper.SelectQuery(selectQueryParametersDataCollection);

            var data = await _apiService.GetData(selectQueryParametersDataCollection);

            return Ok(data);
        }
    }
}
