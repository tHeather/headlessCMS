using headlessCMS.Models.Shared;

namespace headlessCMS.Models.ValueObjects
{
    public class UpdateData
    {
        public Guid RowId { get; set; }
        public string CollectionName { get; set; }
        public List<ColumnWithValue> ColumnsWithValues { get; set; }
    }
}
