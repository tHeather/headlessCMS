using Microsoft.AspNetCore.Mvc;
using headlessCMS.Models.DTOs;
using headlessCMS.Services.Interfaces;
using headlessCMS.Models.ValueObjects;

namespace headlessCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionMetadataService _collectionService;

        public CollectionController(ICollectionMetadataService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionDTO createCollectionDTO)
        {
            var createCollection = new CreateCollection()
            {
                Name = createCollectionDTO.Name,
                Fields = createCollectionDTO.Fields,
            };

            await _collectionService.CreateCollection(createCollection);

            return Ok();
        }

        [HttpGet("get-all")]
        public async Task<IEnumerable<string>> GetCollections()
        {
            return await _collectionService.GetCollectionsNames();
        }

    }
}
