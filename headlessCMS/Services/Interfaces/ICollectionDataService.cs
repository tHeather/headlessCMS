using headlessCMS.Enums;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Models.ValueObjects;

namespace headlessCMS.Services.Interfaces
{
    public interface ICollectionDataService
    {
        public Task<Guid> SaveDraftAsync(InsertData insertData);
        public Task<Guid> PublishDataAsync(Guid draftId, string collectionName);
        public Task SaveDraftAndPublishDataAsync(InsertData insertData);
        public Task<List<dynamic>> GetData(SelectQueryParametersDataCollection selectQueryParametersDataCollection);
        public Task DeleteDataAsync(DeleteData deleteData);
        public Task UpdateDraftAsync(UpdateData updateData);
        public Task UpdateDraftAndPublishDataAsync(UpdateData updateData);
        public Task UnpublishDataAsync(Guid publishedVersionId, string collectionName);
    }
}
