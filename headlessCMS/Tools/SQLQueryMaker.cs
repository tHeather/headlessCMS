using headlessCMS.Constants;
using headlessCMS.Models.Models;
using headlessCMS.Models.Shared;
using System.Text;

namespace headlessCMS.Tools
{
    public static class SQLQueryMaker
    {

        public static string MakeUpdateValuesString(IEnumerable<CollectionField> collectionFields, List<ColumnWithValue> columnsWithValues)
        {
            var values = new StringBuilder();

            foreach (var field in collectionFields)
            {
                var value = columnsWithValues.SingleOrDefault(f => f.Name == field.Name)?.Value;
                var formattedValue = FormatStringValue(value, field.Type);
                values.Append($"{field.Name}={formattedValue},");
            }
            values.Remove(values.Length - 1, 1);

            return values.ToString();
        }

        private static string FormatStringValue(string? value, string type)
        {
            if (value == null) return "NULL";
            
            return type == DatabaseDataType.INT || type == DatabaseDataType.BOOL ? value : $"'{value}'";
        }
    }
}
