using headlessCMS.Models.Services;

namespace headlessCMS.Services.Interfaces
{
    public interface ISqlService
    {
        public Task<Guid> ExecuteInsertQueryOnDataCollection(InsertQueryParametersDataCollection insertQueryParameters);
        public Task ExecuteInsertQueryOnMetadataCollection(InsertQueryParametersMetadataCollection insertQueryParameters);
    }
}
