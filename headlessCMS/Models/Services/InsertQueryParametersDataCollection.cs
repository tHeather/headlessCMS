using headlessCMS.Enums;
using headlessCMS.Models.Shared;

namespace headlessCMS.Models.Services
{
    public class InsertQueryParametersDataCollection
    {
        public string CollectionName { get; set; }
        public DataState DataState { get; set; }
        public List<List<ColumnWithValue>> DataToInsert { get; set; }
    }
}
