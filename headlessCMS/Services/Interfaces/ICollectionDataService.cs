using headlessCMS.Models.ValueObjects;

namespace headlessCMS.Services.Interfaces
{
    public interface ICollectionDataService
    {
        public Task InsertData(InsertData insertData);
        public Task<IEnumerable<dynamic>> GetData(string collectionName);
        public Task DeleteData(DeleteData deleteData);
        public Task UpdateData(UpdateData updateData);
    }
}
