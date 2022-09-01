using headlessCMS.Models.Services;
using headlessCMS.Models.Services.SelectQuery;

namespace headlessCMS.Services.Interfaces
{
    public interface ISqlApiService
    {
        public Task<Guid> ExecuteInsertQueryAsync(InsertQueryParametersDataCollection insertQueryParameters);

        public Task<Guid> ExecuteDeleteQueryAsync(DeleteQueryParametersDataCollection deleteQueryParameters);

        public Task<List<dynamic>> ExecuteSelectQueryAsync(SelectQueryParametersDataCollection selectQueryParameters);
    }
}
