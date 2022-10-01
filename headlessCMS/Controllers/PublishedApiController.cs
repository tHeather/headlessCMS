using headlessCMS.Filters;
using headlessCMS.Models.Services.Api.InsertQuery;
using headlessCMS.Models.Services.InsertQuery;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("data")]
    [RemoveDbSchemaActionFilter("collectionName")]
    public class PublishedAPIController : ControllerBase
    {
        private readonly ISqlDataService _sqlDataService;

        public PublishedAPIController(ISqlDataService sqlDataService)
        {
            _sqlDataService = sqlDataService;
        }

        [HttpPost("get")]
        [RemoveDbSchemaFromSelectQueryActionFilter("queryParameters")]
        public async Task<IActionResult> GetDataAsync(SelectQueryParametersDataCollection queryParameters)
        {
            var data = await _sqlDataService.ExecuteSelectQueryAsync(queryParameters);
            return Ok(data);
        }

        [HttpPost("{collectionName}/insert")]
        public async Task<IActionResult> InsertDataAsync
            (string collectionName, InsertDataRow dataToInsert)
        {
            var insertData = new InsertQueryParameters()
            {
                CollectionName = collectionName,
                DataToInsert = dataToInsert
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

        //[HttpDelete("{collectionName}/{draftId}")]
        //public async Task<IActionResult> DeleteDataAsync(string collectionName, Guid draftId)
        //{

        //    var deleteData = new DeleteData() 
        //    {
        //        CollectionName = collectionName,
        //        DraftId = draftId
        //    };

        //    await _apiService.DeleteDataAsync(deleteData);

        //    return Ok();
        //}

        //[HttpPut("{collectionName}/{id}")]
        //public async Task<IActionResult> UpdateDataAsync(
        //    string collectionName, Guid id, [FromBody] List<ColumnWithValue> ColumnsWithValues)
        //{

        //    var updateData = new UpdateData()
        //    {
        //        CollectionName = collectionName,
        //        RowId = id,
        //        ColumnsWithValues= ColumnsWithValues
        //    };

        //    await _apiService.UpdateDraftAsync(updateData);

        //    return Ok();
        //}

        //[HttpPost("publish/{collectionName}")]
        //public async Task<IActionResult> PublishDataAsync(string collectionName, [FromBody] Guid draftId)
        //{

        //    await _apiService.PublishDataAsync(draftId, collectionName);

        //    return Ok();
        //}

        //[HttpPost("unpublish/{collectionName}")]
        //public async Task<IActionResult> UnpublishDataAsync(string collectionName, [FromBody] Guid publishedVersionId)
        //{

        //    await _apiService.UnpublishDataAsync(publishedVersionId, collectionName);

        //    return Ok();
        //}

    }
}
