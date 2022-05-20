using headlessCMS.Enums;
using headlessCMS.Models.Shared;

namespace headlessCMS.Models.ValueObjects
{
    public class InsertData
    {
        public string CollectionName { get; set; }
        public List<ColumnWithValue> ColumnsWithValues { get; set; }
    }
}
