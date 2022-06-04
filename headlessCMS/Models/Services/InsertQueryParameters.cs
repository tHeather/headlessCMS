using headlessCMS.Enums;
using headlessCMS.Models.Models;
using headlessCMS.Models.Shared;

namespace headlessCMS.Models.Services
{
    public class InsertQueryParameters
    {
        public string CollectionName { get; set; }
        public DataStates DataState { get; set; }
        public List<ColumnWithValue> DataToInsert { get; set; }
    }
}
