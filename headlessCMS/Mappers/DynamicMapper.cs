using headlessCMS.Models.Shared;

namespace headlessCMS.Mappers
{
    public static class DynamicMapper
    {
        public static List<ColumnWithValue> DynamicToColumnsWithValuesList(dynamic dynamicData)
        {
            var mappedData = new List<ColumnWithValue>();

            foreach (var row in dynamicData)
            {
                mappedData.Add(new ColumnWithValue
                {
                    Name = row.Key,
                    Value = row.Value != null ? row.Value.ToString() : "NULL"
                });
            }

            return mappedData;
        }
    }
}
