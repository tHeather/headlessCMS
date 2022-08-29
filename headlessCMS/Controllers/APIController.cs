using headlessCMS.Models.Shared;
using headlessCMS.Models.ValueObjects;
using headlessCMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using headlessCMS.Models.Services.SelectQuery;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class APIController : ControllerBase
    {
        private readonly ICollectionDataService _collectionDataService;

        public APIController(ICollectionDataService collectionDataService)
        {
            _collectionDataService = collectionDataService;
        }

        [HttpPost("getdata")]
        public async Task<IActionResult> GetDataAsync(SelectQueryParametersDataCollection selectQueryParametersDataCollection)
        {
            var data = await _collectionDataService.GetData(selectQueryParametersDataCollection);

            return Ok(data);
        }

        [HttpPost("{collectionName}")]
        public async Task<IActionResult> InsertDataAsync
            (string collectionName, [FromBody] List<List<ColumnWithValue>> columnsWithValues)
        {
            var insertData  = new InsertData() 
            {
                CollectionName = collectionName,
                ColumnsWithValues = columnsWithValues
            };

            await _collectionDataService.SaveDraftAsync(insertData);

            return Ok();
        }

        [HttpDelete("{collectionName}/{draftId}")]
        public async Task<IActionResult> DeleteDataAsync(string collectionName, Guid draftId)
        {

            var deleteData = new DeleteData() 
            {
                CollectionName = collectionName,
                DraftId = draftId
            };

            await _collectionDataService.DeleteDataAsync(deleteData);

            return Ok();
        }

        [HttpPut("{collectionName}/{id}")]
        public async Task<IActionResult> UpdateDataAsync(
            string collectionName, Guid id, [FromBody] List<ColumnWithValue> ColumnsWithValues)
        {

            var updateData = new UpdateData()
            {
                CollectionName = collectionName,
                RowId = id,
                ColumnsWithValues= ColumnsWithValues
            };

            await _collectionDataService.UpdateDraftAsync(updateData);

            return Ok();
        }

        [HttpPost("publish/{collectionName}")]
        public async Task<IActionResult> PublishDataAsync(string collectionName, [FromBody] Guid draftId)
        {

            await _collectionDataService.PublishDataAsync(draftId, collectionName);

            return Ok();
        }

        [HttpPost("unpublish/{collectionName}")]
        public async Task<IActionResult> UnpublishDataAsync(string collectionName, [FromBody] Guid publishedVersionId)
        {

            await _collectionDataService.UnpublishDataAsync(publishedVersionId, collectionName);

            return Ok();
        }

    }
}
