using headlessCMS.Enums;
using headlessCMS.Models.Shared;
using headlessCMS.Models.ValueObjects;
using headlessCMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{collectionName}")]
        public async Task<IActionResult> GetData(string collectionName, DataStates dataState)
        {
            var data = await _collectionDataService.GetData(collectionName, dataState);

            return Ok(data);
        }

        [HttpPost("{collectionName}")]
        public async Task<IActionResult> InsertData
            (string collectionName, [FromBody] List<List<ColumnWithValue>> columnsWithValues)
        {
            var insertData  = new InsertData() 
            {
                CollectionName = collectionName,
                ColumnsWithValues = columnsWithValues
            };

            await _collectionDataService.SaveDraft(insertData);

            return Ok();
        }

        [HttpDelete("{collectionName}/{draftId}")]
        public async Task<IActionResult> DeleteData(string collectionName, Guid draftId)
        {

            var deleteData = new DeleteData() 
            {
                CollectionName = collectionName,
                DraftId = draftId
            };

            await _collectionDataService.DeleteData(deleteData);

            return Ok();
        }

        [HttpPut("{collectionName}/{id}")]
        public async Task<IActionResult> UpdateData(
            string collectionName, Guid id, [FromBody] List<ColumnWithValue> ColumnsWithValues)
        {

            var updateData = new UpdateData()
            {
                CollectionName = collectionName,
                RowId = id,
                ColumnsWithValues= ColumnsWithValues
            };

            await _collectionDataService.UpdateDraft(updateData);

            return Ok();
        }

        [HttpPost("publish/{collectionName}")]
        public async Task<IActionResult> PublishData(string collectionName, [FromBody] Guid draftId)
        {

            await _collectionDataService.PublishData(draftId, collectionName);

            return Ok();
        }

        [HttpPost("unpublish/{collectionName}")]
        public async Task<IActionResult> UnpublishData(string collectionName, [FromBody] Guid publishedVersionId)
        {

            await _collectionDataService.UnpublishData(publishedVersionId, collectionName);

            return Ok();
        }

    }
}
