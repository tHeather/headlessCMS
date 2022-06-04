using headlessCMS.Models.Services;
using headlessCMS.Models.Shared;

namespace headlessCMS.Services.Interfaces
{
    public interface ISqlService
    {
        public Task<Guid> ExecuteInsertQuery(InsertQueryParameters insertQueryParameters);
    }
}
