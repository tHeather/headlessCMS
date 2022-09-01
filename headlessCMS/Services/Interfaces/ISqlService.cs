using headlessCMS.Models.Models;

namespace headlessCMS.Services.Interfaces
{
    public interface ISqlService
    {
        protected Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionNameAsync(string collectionName);
    }
}
