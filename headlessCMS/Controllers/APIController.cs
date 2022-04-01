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
        public async Task<IActionResult> GetData(string collectionName)
        {
            var data = await _collectionDataService.GetData(collectionName);

            return Ok(data);
        }

        [HttpPost("{collectionName}")]
        public async Task<IActionResult> InsertData(
            string collectionName,[FromBody] Dictionary<string, string> ColumnsWithValues
            )
        {

            var insertData  = new InsertData() 
            {
                CollectionName = collectionName,
                ColumnsWithValues = ColumnsWithValues
            };

            await _collectionDataService.InsertData(insertData);

            return Ok();
        }

        [HttpDelete("{collectionName}/{id}")]
        public async Task<IActionResult> DeleteData(string collectionName, Guid id)
        {

            var deleteData = new DeleteData() 
            {
                CollectionName = collectionName,
                RowId = id
            };

            await _collectionDataService.DeleteData(deleteData);

            return Ok();
        }

        [HttpPut("{collectionName}/{id}")]
        public async Task<IActionResult> UpdateData(
            string collectionName, Guid id, [FromBody] Dictionary<string, string> ColumnsWithValues
            )
        {

            var updateData = new UpdateData()
            {
                CollectionName = collectionName,
                RowId = id,
                ColumnsWithValues= ColumnsWithValues
            };

            await _collectionDataService.UpdateData(updateData);

            return Ok();
        }

    }
}
