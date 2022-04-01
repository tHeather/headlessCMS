namespace headlessCMS.Models.ValueObjects
{
    public class InsertData
    {
        public string CollectionName { get; set; }
        public Dictionary<string, string> ColumnsWithValues { get; set; }
    }
}
