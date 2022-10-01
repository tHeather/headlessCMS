using headlessCMS.Filters;
using headlessCMS.Models.Services.Api.InsertQuery;
using headlessCMS.Models.Services.InsertQuery;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("draft")]
    [AddDraftDbSchemaActionFilter("collectionName")]
    public class DraftApiController : Controller
    {
        private readonly ISqlDataService _sqlDataService;

        public DraftApiController(ISqlDataService sqlDataService)
        {
            _sqlDataService = sqlDataService;
        }

        [HttpPost("get")]
        [AddDraftDbSchemaToSelectQueryActionFilter("queryParameters")]
        public async Task<IActionResult> GetDataAsync(SelectQueryParametersDataCollection queryParameters)
        {
            var data = await _sqlDataService.ExecuteSelectQueryAsync(queryParameters);
            return Ok(data);
        }

        [HttpPost("{collectionName}/insert")]
        public async Task<IActionResult> InsertDataAsync
            (string collectionName, InsertDataRow columnsWithValues)
        {
            var insertData = new InsertQueryParameters()
            {
                CollectionName = collectionName,
                DataToInsert = columnsWithValues
            };

            var savedDataId = await _sqlDataService.InsertDataAsync(insertData);

            return Ok(savedDataId);
        }

        [HttpPost("{collectionName}/insert-many")]
        public async Task<IActionResult> InsertDataAsync
            (string collectionName, List<InsertDataRow> dataToInsert)
        {
            var insertData = new InsertManyQueryParameters()
            {
                CollectionName = collectionName,
                DataToInsert = dataToInsert
            };

            var insertedIds = await _sqlDataService.InsertManyDataAsync(insertData);

            return Ok(insertedIds);
        }

    }
}
