using headlessCMS.Enums;
using headlessCMS.Models.ValueObjects;

namespace headlessCMS.Services.Interfaces
{
    public interface ICollectionDataService
    {
        public Task<Guid> SaveDraft(InsertData insertData);
        public Task<Guid?> PublishData(Guid draftId, string collectionName);
        public Task SaveDraftAndPublishData(InsertData insertData);
        public Task<IEnumerable<dynamic>> GetData(string collectionName, DataStates dataDtate);
        public Task DeleteData(DeleteData deleteData);
        public Task UpdateDraft(UpdateData updateData);
        public Task UpdateDraftAndPublishData(UpdateData updateData);
        public Task UnpublishData(Guid publishedVersionId, string collectionName);
    }
}
