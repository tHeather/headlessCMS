using headlessCMS.Enums;
using headlessCMS.Models.Shared;

namespace headlessCMS.Models.Services.InsertQuery
{
    public class InsertQueryParameters
    {
        public string CollectionName { get; set; }

        public List<ColumnWithValue> DataToInsert { get; set; }
    }
}
