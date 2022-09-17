using headlessCMS.Models.Shared;
using headlessCMS.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Services;
using headlessCMS.Models.Services.InsertQuery;
using headlessCMS.Filters;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("data")]
    [RemoveDbSchemaActionFilter("collectionName")]
    public class PublishedAPIController : ControllerBase
    {
        private readonly ApiService _apiService;

        public PublishedAPIController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost("get")]
        [RemoveDbSchemaFromSelectQueryActionFilter("queryParameters")]
        public async Task<IActionResult> GetDataAsync(SelectQueryParametersDataCollection queryParameters)
        {
            var data = await _apiService.GetData(queryParameters);
            return Ok(data);
        }

        [HttpPost("{collectionName}/insert")]
        public async Task<IActionResult> InsertDataAsync
            (string collectionName, [FromBody] List<ColumnWithValue> columnsWithValues)
        {
            var insertData  = new InsertQueryParameters() 
            {
                CollectionName = collectionName,
                DataToInsert = columnsWithValues
            };

            var savedDataId = await _apiService.InsertDataAsync(insertData);

            return Ok(savedDataId);
        }

    //    [HttpPost("insert-many")]
    //    public async Task<IActionResult> InsertDataAsync
    //(string collectionName, [FromBody] List<List<ColumnWithValue>> columnsWithValues)
    //    {
    //        var insertData = new InsertData()
    //        {
    //            CollectionName = collectionName,
    //            ColumnsWithValues = columnsWithValues
    //        };

    //        await _apiService.SaveDraftAsync(insertData);

    //        return Ok();
    //    }

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
