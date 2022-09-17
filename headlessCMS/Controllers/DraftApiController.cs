using headlessCMS.Mappers;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Models.Shared;
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

        [HttpPost("get")]
        public async Task<IActionResult> GetDataAsync(SelectQueryParametersDataCollection selectQueryParametersDataCollection)
        {
            DraftApiRequestMapper.SelectQuery(selectQueryParametersDataCollection);

            var data = await _apiService.GetData(selectQueryParametersDataCollection);

            return Ok(data);
        }

        //[HttpPost("{collectionName}/insert")]
        //public async Task<IActionResult> InsertDataAsync
        //    (string collectionName, [FromBody] List<ColumnWithValue> columnsWithValues)
        //{
        //    var insertData  = new InsertQueryParameters() 
        //    {
        //        CollectionName = collectionName,
        //        DataToInsert = columnsWithValues
        //    };

        //    var savedDataId = await _apiService.InsertPublishedDataAsync(insertData);

        //    return Ok(savedDataId);
        //}
    }
}
