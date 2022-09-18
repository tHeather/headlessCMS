using headlessCMS.Models.Services.Api.InsertQuery;

namespace headlessCMS.Models.Services.InsertQuery
{
    public class InsertQueryParameters
    {
        public string CollectionName { get; set; }

        public InsertDataRow DataToInsert { get; set; }
    }
}
