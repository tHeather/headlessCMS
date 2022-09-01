using headlessCMS.Models.Services;

namespace headlessCMS.Services.Interfaces
{
    public interface ISqlCmsService
    {
        public Task ExecuteInsertQueryAsync(InsertQueryParametersMetadataCollection insertQueryParameters);
    }
}
