using headlessCMS.Enums;
using headlessCMS.Models.Shared;

namespace headlessCMS.Models.Services.InsertQuery
{
    public class InsertManyQueryParameters
    {
        public string CollectionName { get; set; }
        public DataState DataState { get; set; }
        public List<List<ColumnWithValue>> DataToInsert { get; set; }
    }
}
