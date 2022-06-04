using Dapper;
using headlessCMS.Constants;
using headlessCMS.Dictionary;
using headlessCMS.Models.Models;
using headlessCMS.Models.Services;
using headlessCMS.Models.Shared;
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

        public async Task<Guid> ExecuteInsertQuery(InsertQueryParameters insertQueryParameters)
        {
            var collectionFields = await GetCollectionFieldsByCollectionName(insertQueryParameters.CollectionName);
            if (!collectionFields.Any()) return Guid.Empty;

            var values = new StringBuilder();
            var columns = new StringBuilder();
            var parameters = new DynamicParameters();

            foreach (var field in collectionFields)
            {
                var value = insertQueryParameters.DataToInsert.SingleOrDefault(d => d.Name == field.Name)?.Value;
                if (value == null) continue;
                    
                var dapperType = DataTypesMapper.MapDatabaseTypeToDapper[field.Type];

                parameters.Add($"@{field.Name}", value, dapperType);
                values.Append($"@{field.Name},");
                columns.Append($"{field.Name},");
            }

            values.Remove(values.Length - 1, 1);
            columns.Remove(columns.Length - 1, 1);
            
            var query = @$"INSERT INTO {insertQueryParameters.CollectionName} 
                           ({ReservedColumns.DATA_STATE}, {ReservedColumns.PUBLISHED_VERSION_ID}, {columns}) 
                           OUTPUT inserted.id
                           VALUES ({(int)insertQueryParameters.DataState}, NULL, {values});";

            return await _dbConnection.ExecuteScalarAsync<Guid>(query, parameters);
        }

        private async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionName(string collectionName)
        {
            var query = $"SELECT * FROM {ReservedTables.COLLECTION_FIELDS} WHERE collectionName = @collectionName;";

            return await _dbConnection.QueryAsync<CollectionField>(query, new { collectionName });
        }

        private List<ColumnWithValue> FilterOutReservedColumns(List<ColumnWithValue> columnsWithValues) // WILL BE USE FOR TABLE CREATION
        {
            var filteredColumnsWithValues = new List<ColumnWithValue>(columnsWithValues);

            filteredColumnsWithValues.RemoveAll(columnWithValue => ReservedColumns.ReservedDataTableColumnsList.Contains(columnWithValue.Name));

            return filteredColumnsWithValues;
        }
    }
}
