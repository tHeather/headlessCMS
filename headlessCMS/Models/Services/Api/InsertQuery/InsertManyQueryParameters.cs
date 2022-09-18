using headlessCMS.Enums;
using headlessCMS.Models.Services.Api.InsertQuery;

namespace headlessCMS.Models.Services.InsertQuery
{
    public class InsertManyQueryParameters
    {
        public string CollectionName { get; set; }

        public List<InsertDataRow> DataToInsert { get; set; }
    }
}
