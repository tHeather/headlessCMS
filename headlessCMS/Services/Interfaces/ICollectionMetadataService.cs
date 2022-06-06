using headlessCMS.Models.DTOs;
using headlessCMS.Models.Models;
using headlessCMS.Models.ValueObjects;

namespace headlessCMS.Services.Interfaces
{
    public interface ICollectionMetadataService
    {
        public  Task CreateCollectionAsync(CreateCollection reateCollection);
        public Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionNameAsync(string name);
        public Task<IEnumerable<string>> GetCollectionsNames();
    }
}
