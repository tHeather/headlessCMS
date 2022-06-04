using headlessCMS.Constants;
using headlessCMS.Models.Models;
using headlessCMS.Models.Services;
using headlessCMS.Models.Shared;
using System.Text;

namespace headlessCMS.Tools
{
    public static class SQLQueryMaker
    {
        public static ColumnsAndValuesStrings MakeInsertValuesStrings(IEnumerable<CollectionField> collectionFields, List<ColumnWithValue> dataToInsert)
        {
            var values = new StringBuilder();
            var columns = new StringBuilder();
            var filteredDataToInsert = FilterOutReservedColumns(dataToInsert);

            foreach (var field in collectionFields)
            {
                var value = filteredDataToInsert.SingleOrDefault(f => f.Name == field.Name)?.Value;
                var formattedValue = FormatStringValue(value, field.Type);
                values.Append($"{formattedValue},");
                columns.Append($"{field.Name},");
            }

            values.Remove(values.Length - 1, 1);
            columns.Remove(columns.Length - 1, 1);

            return new ColumnsAndValuesStrings
            {
                Values = values.ToString(),
                Columns = columns.ToString()
            };
        }

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

        private static List<ColumnWithValue> FilterOutReservedColumns(List<ColumnWithValue> columnsWithValues)
        {
            var filteredColumnsWithValues = new List<ColumnWithValue>(columnsWithValues);

            filteredColumnsWithValues.RemoveAll(columnWithValue => ReservedColumns.ReservedDataTableColumnsList.Contains(columnWithValue.Name)) ;

            return filteredColumnsWithValues;
        }

        private static string FormatStringValue(string? value, string type)
        {
            if (value == null) return "NULL";
            
            return type == DatabaseDataType.INT || type == DatabaseDataType.BOOL ? value : $"'{value}'";
        }
    }
}
