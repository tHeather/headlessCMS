using headlessCMS.Models.DTOs;
using headlessCMS.Models.Models;
using headlessCMS.Models.ValueObjects;

namespace headlessCMS.Services.Interfaces
{
    public interface ICollectionMetadataService
    {
        public  Task CreateCollection(CreateCollection reateCollection);
        public Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionName(string name);
        public Task<IEnumerable<string>> GetCollectionsNames();
    }
}
