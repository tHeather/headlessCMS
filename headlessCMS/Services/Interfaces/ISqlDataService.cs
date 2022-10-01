using headlessCMS.Models.Services;
using headlessCMS.Models.Services.InsertQuery;
using headlessCMS.Models.Services.SelectQuery;

namespace headlessCMS.Services.Interfaces
{
    public interface ISqlDataService
    {
        public Task<Guid> InsertDataAsync(InsertQueryParameters insertQueryParameters);

        public Task<IEnumerable<Guid>> InsertManyDataAsync(InsertManyQueryParameters insertQueryParameters);

        public Task<List<dynamic>> ExecuteSelectQueryAsync(SelectQueryParametersDataCollection selectQueryParameters);

        //public Task<Guid> ExecuteDeleteQueryAsync(DeleteQueryParametersDataCollection deleteQueryParameters);
    }
}
