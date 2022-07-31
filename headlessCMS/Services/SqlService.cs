using Dapper;
using headlessCMS.Constants;
using headlessCMS.Dictionary;
using headlessCMS.Mappers;
using headlessCMS.Models.Models;
using headlessCMS.Models.Services;
using headlessCMS.Models.Services.Select;
using headlessCMS.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace headlessCMS.Services
{
    public class SqlService : ISqlService
    {
        private readonly SqlConnection _dbConnection;

        public SqlService(SqlConnection connection)
        {
            _dbConnection = connection;
        }

        public async Task<Guid> ExecuteInsertQueryOnDataCollectionAsync(InsertQueryParametersDataCollection insertQueryParameters)
        {
            var collectionFields = await GetCollectionFieldsByCollectionNameAsync(insertQueryParameters.CollectionName);
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

        public async Task ExecuteInsertQueryOnMetadataCollectionAsync(InsertQueryParametersMetadataCollection insertQueryParameters)
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

            var query = @$"INSERT INTO {insertQueryParameters.CollectionName} ({columns}) VALUES {values};";

            await _dbConnection.ExecuteAsync(query, parameters);
        }

        public async Task<Guid> ExecuteDeleteQueryOnDataCollectionAsync(DeleteQueryParametersDataCollection deleteQueryParameters)
        {
            var collectionFields = await GetCollectionFieldsByCollectionNameAsync(deleteQueryParameters.CollectionName);
            if (!collectionFields.Any()) return Guid.Empty;

            var conditions = new StringBuilder();
            var parameters = new DynamicParameters();

            foreach (var (idAndDataState, index) in deleteQueryParameters.IdsAndDataStates.Select((idAndDataState, index) => (idAndDataState, index)))
            {
                var idParameterName = $"@{index}idName";
                var dataStateParameterName = $"@{index}dataState";

                var separator = deleteQueryParameters.IdsAndDataStates.Count == index + 1 ? "" : "OR";
                conditions.Append(@$"({DataCollectionReservedFields.ID} = {idParameterName} AND {DataCollectionReservedFields.DATA_STATE} = {dataStateParameterName}) {separator}");
                parameters.Add(idParameterName, idAndDataState.Id, DbType.Guid);
                parameters.Add(dataStateParameterName, (int)idAndDataState.DataState, DbType.Int32);
            }

            return await _dbConnection.ExecuteScalarAsync<Guid>(@$"DELETE FROM {deleteQueryParameters.CollectionName}
                                                                   OUTPUT deleted.Id
                                                                   WHERE {conditions};", parameters);
        }

        public async Task<List<dynamic>> ExecuteSelectQueryOnDataCollectionAsync(SelectQueryParametersDataCollection selectQueryParameters)
        {
            if (! await CheckIfCollectionsAndColumnsExistsInDatabase(selectQueryParameters.FieldsFilters)) return new List<dynamic>();

            var selectedFields = MakeSelectedFieldsQueryPart(selectQueryParameters.SelctedFields);
            var filtersString = MakeFilterQueryPart(selectQueryParameters.FieldsFilters);
            return new List<dynamic>();
        }

        private string MakeSelectedFieldsQueryPart(List<SelectSelectedField> selctedFields)
        {
            var query = new StringBuilder("SELECT ");

            foreach (var field in selctedFields)
            {
                query.Append($"{field.CollectionName}.{field.FieldName}, ");
            }

            query.Remove(query.Length - 2, 2);

            return query.ToString();
        }

        private async Task<bool> CheckIfCollectionsAndColumnsExistsInDatabase(List<SelectFiltersField> fieldsFilters)
        {
            // TODO: check data also from join and select

            var collectionWithColumnsNames = fieldsFilters
                                                .GroupBy(field => field.CollectionName,
                                                        field => field.FieldName)
                                                .Select(groupedFields => new CollectionWithColumnsNames()
                                                {
                                                    CollectionName = groupedFields.Key,
                                                    ColumnsNames = groupedFields.ToList()
                                                });


            foreach (var collection in collectionWithColumnsNames)
            {
                var collectionFromDB = await GetCollectionFieldsByCollectionNameAsync(collection.CollectionName);

                if (collectionFromDB == null) return false;

                var fieldsNamesFromDB = collectionFromDB.Select(field => field.Name).ToList();

                var areAllColumnsExistInDb = collection.ColumnsNames.All(fieldName => fieldsNamesFromDB.Contains(fieldName));

                if (!areAllColumnsExistInDb) return false;
            }

            return true;
        }

        private string PrepareOperationSign(string? previousOperation, string currentOperation)
        {
            if (previousOperation == null) return "(";

            return currentOperation switch
            {
                LogicalOperations.AND => LogicalOperations.AND,
                LogicalOperations.OR => $") {LogicalOperations.OR} (",
                _ => string.Empty,
            };
        }

        private string MakeFilterQueryPart(List<SelectFiltersField> fieldsFilters)
        {
            var query = new StringBuilder("WHERE ");
            var parameters = new DynamicParameters();
            string? previousOperation = null;

            foreach (var field in fieldsFilters)
            {
                foreach (var filter in field.Filters)
                {
                    var collectionAndFieldName = $"{field.CollectionName}.{field.FieldName}";
                    var filterSign = SelectFiltersMapper.MapFilterToSign[filter.Type];

                    var operationSign = PrepareOperationSign(previousOperation, field.Operation);
                    if (operationSign == string.Empty) return string.Empty;

                    query.Append($" {operationSign} {collectionAndFieldName} {filterSign} @{collectionAndFieldName}");
                    parameters.Add($"@{collectionAndFieldName}", filter.Value);
                }
                previousOperation = field.Operation;
            }

            return query.Append(')').ToString();
        }


        private async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionNameAsync(string collectionName)
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
