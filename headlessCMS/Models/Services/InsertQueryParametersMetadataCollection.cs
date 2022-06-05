using headlessCMS.Models.Shared;

namespace headlessCMS.Models.Services
{
    public class InsertQueryParametersMetadataCollection
    {
        public string CollectionName { get; set; }
        public List<List<ColumnWithValue>> DataToInsert { get; set; }
    }
}
