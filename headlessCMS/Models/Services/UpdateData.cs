namespace headlessCMS.Models.ValueObjects
{
    public class UpdateData
    {
        public Guid RowId { get; set; }
        public string CollectionName { get; set; }
        public Dictionary<string, string> ColumnsWithValues { get; set; }
    }
}
