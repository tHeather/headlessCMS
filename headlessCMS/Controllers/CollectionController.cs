using Microsoft.AspNetCore.Mvc;
using headlessCMS.Models.DTOs;
using headlessCMS.Models.ValueObjects;
using headlessCMS.Services;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollectionController : ControllerBase
    {
        private readonly CollectionMetadataService _collectionService;

        public CollectionController(CollectionMetadataService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCollectionAsync([FromBody] CreateCollectionDTO createCollectionDTO)
        {
            var createCollection = new CreateCollection()
            {
                Name = createCollectionDTO.Name,
                Fields = createCollectionDTO.Fields,
            };

            await _collectionService.CreateCollectionAsync(createCollection);

            return Ok();
        }

        [HttpGet("get-all")]
        public async Task<IEnumerable<string>> GetCollectionsAsync()
        {
            return await _collectionService.GetCollectionsNames();
        }

    }
}
