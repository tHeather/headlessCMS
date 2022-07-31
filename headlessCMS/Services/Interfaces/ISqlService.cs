using headlessCMS.Models.Services;

namespace headlessCMS.Services.Interfaces
{
    public interface ISqlService
    {
        public Task<Guid> ExecuteInsertQueryOnDataCollectionAsync(InsertQueryParametersDataCollection insertQueryParameters);

        public Task ExecuteInsertQueryOnMetadataCollectionAsync(InsertQueryParametersMetadataCollection insertQueryParameters);

        public Task<Guid> ExecuteDeleteQueryOnDataCollectionAsync(DeleteQueryParametersDataCollection deleteQueryParameters);

        public Task<List<dynamic>> ExecuteSelectQueryOnDataCollectionAsync(SelectQueryParametersDataCollection selectQueryParameters);
    }
}
