using Dapper;
using headlessCMS.Constants;
using headlessCMS.Dictionary;
using headlessCMS.Models.Models;
using headlessCMS.Models.Services;
using headlessCMS.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace headlessCMS.Services
{
    public class SqlService: ISqlService
    {
        private readonly SqlConnection _dbConnection;

        public SqlService(SqlConnection connection)
        {
            _dbConnection = connection;
        }

        public async Task<Guid> ExecuteInsertQueryOnDataCollection(InsertQueryParametersDataCollection insertQueryParameters)
        {
            var collectionFields = await GetCollectionFieldsByCollectionName(insertQueryParameters.CollectionName);
            if (!collectionFields.Any()) return Guid.Empty;

            var values = new StringBuilder();
            var columns = string.Join(",", collectionFields.Select(f => f.Name));
            var parameters = new DynamicParameters();
            var beginningOfValues = $"({(int)insertQueryParameters.DataState}, NULL,";

            foreach (var (rowToInsert, index) in insertQueryParameters.DataToInsert.Select((rowToInsert, index) => (rowToInsert, index)))
            {
                values.Append(beginningOfValues);
                foreach (var field in collectionFields)
                {
                    var value = rowToInsert.SingleOrDefault(d => d.Name == field.Name)?.Value;
                    if (value == null) continue;

                    var dapperType = DataTypesMapper.MapDatabaseTypeToDapper[field.Type];

                    parameters.Add($"@{index}{field.Name}", value, dapperType);
                    values.Append($"@{index}{field.Name},");
                }
                if (values[^1] == '(')
                {
                    values.Remove(values.Length - 1, 1);
                }
                else
                {
                    var isLastRow = insertQueryParameters.DataToInsert.Count == index + 1;
                    values.Replace(",", isLastRow ? ")" : "),", values.Length - 1, 1);
                }
            }

            var query = @$"INSERT INTO {insertQueryParameters.CollectionName} 
                           ({DataCollectionReservedFields.DATA_STATE}, {DataCollectionReservedFields.PUBLISHED_VERSION_ID}, {columns}) 
                           OUTPUT inserted.id
                           VALUES {values};";

            return await _dbConnection.ExecuteScalarAsync<Guid>(query, parameters);
        }

        public async Task ExecuteInsertQueryOnMetadataCollection(InsertQueryParametersMetadataCollection insertQueryParameters)
        {
           var reservedTableFields = ReservedTables.GetReservedTableFields(insertQueryParameters.CollectionName);
           if (reservedTableFields == null) return;

            var values = new StringBuilder();
            var columns = string.Join(",", reservedTableFields);
            var parameters = new DynamicParameters();

            foreach (var (rowToInsert, index) in insertQueryParameters.DataToInsert.Select((rowToInsert, index) => (rowToInsert, index)))
            {
                values.Append('(');
                foreach (var fieldName in reservedTableFields)
                {
                    var value = rowToInsert.SingleOrDefault(d => d.Name == fieldName)?.Value;
                    if (value == null) continue;

                    parameters.Add($"@{index}{fieldName}", value);
                    values.Append($"@{index}{fieldName},");
                }
                if(values[^1] == '(')
                {
                    values.Remove(values.Length - 1, 1);
                }
                else
                {
                    var isLastRow = insertQueryParameters.DataToInsert.Count == index + 1;
                    values.Replace(",", isLastRow ? ")" : ")," , values.Length - 1, 1);
                }
            }

            var query = @$"INSERT INTO {insertQueryParameters.CollectionName} ({columns}) VALUES {values};";

            await _dbConnection.ExecuteAsync(query, parameters);
        }

        private async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionName(string collectionName)
        {
            var query = $"SELECT * FROM {ReservedTables.COLLECTION_FIELDS} WHERE collectionName = @collectionName;";

            return await _dbConnection.QueryAsync<CollectionField>(query, new { collectionName });
        }

        //private List<ColumnWithValue> FilterOutReservedColumns(List<ColumnWithValue> columnsWithValues) // WILL BE USE FOR TABLE CREATION
        //{
        //    var filteredColumnsWithValues = new List<ColumnWithValue>(columnsWithValues);

        //    filteredColumnsWithValues.RemoveAll(columnWithValue => ReservedColumns.ReservedDataTableColumnsList.Contains(columnWithValue.Name));

        //    return filteredColumnsWithValues;
        //}
    }
}
