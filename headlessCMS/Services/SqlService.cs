using Dapper;
using headlessCMS.Constants;
using headlessCMS.Constants.TablesMetadata;
using headlessCMS.Dictionary;
using headlessCMS.Mappers;
using headlessCMS.Models.Models;
using headlessCMS.Models.Services;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;

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
            var areCollectionsAndFieldsExistInDatabase = await CheckIfCollectionsAndColumnsExistInDatabase
                                                               (
                                                                 selectQueryParameters.FieldsFilters,
                                                                 selectQueryParameters.SelctedFields,
                                                                 selectQueryParameters.Joins,
                                                                 selectQueryParameters.From,
                                                                 selectQueryParameters.Orders
                                                               );

            if (!areCollectionsAndFieldsExistInDatabase)
            {
                return new List<dynamic>();
            }

            var query = new StringBuilder();
            var parameters = new DynamicParameters();

            var selectedFieldsQuery = MakeSelectedFieldsQueryPart(selectQueryParameters.SelctedFields);
            query.Append(selectedFieldsQuery);

            var fromQuery = MakeFromQueryPart(selectQueryParameters.From);
            query.Append(fromQuery);

            if (selectQueryParameters.Joins.Any())
            {
                var joinQuery = MakeJoinQueryPart(selectQueryParameters.Joins);
                query.Append(joinQuery);
            }

            if (selectQueryParameters.FieldsFilters.Any())
            {
                var filterQueryAndParameters = MakeFilterQueryPart(selectQueryParameters.FieldsFilters);
                query.Append(filterQueryAndParameters.Query);
                parameters.AddDynamicParams(filterQueryAndParameters.Parameters);
            }

            var orderQuery = MakeOrderQueryPart(selectQueryParameters.Orders);
            query.Append(orderQuery);

            if(selectQueryParameters.Pagination != null)
            {
                var paginationQuery = MakePaginationQueryPart(selectQueryParameters.Pagination);
                query.Append(paginationQuery);
            }

            var results = await _dbConnection.QueryAsync(query.ToString(), parameters);
            return results.ToList();
        }

        private StringBuilder MakePaginationQueryPart(SelectQueryPagination pagination)
        {
            var rowsToskip = pagination.PageSize * pagination.PageNumber;
            return new StringBuilder($" OFFSET {rowsToskip} ROWS FETCH NEXT {pagination.PageSize} ROWS ONLY");
        }

        private StringBuilder MakeOrderQueryPart(List<SelectQueryOrder> orders)
        {
            var query = new StringBuilder(" ORDER BY");

            if(orders.Any())
            {
                foreach (var order in orders)
                {
                    if (!OrderTypes.OrderTypesList.Contains(order.Type, StringComparer.OrdinalIgnoreCase)) continue;
                    query.Append($" {order.CollectionName}.{order.FieldName} {order.Type},");
                }

                query.Remove(query.Length - 1, 1);
            }
            else
            {
                query.Append($" id");
            }

            return query;
        }

        private StringBuilder MakeFromQueryPart(string collectionName)
        {
            return new StringBuilder($" FROM {collectionName} ");
        }

        private StringBuilder MakeJoinQueryPart(List<SelectQueryJoin> joins)
        {
            var query = new StringBuilder(" ");

            foreach (var join in joins)
            {
                if (!JoinTypes.JoinTypesList.Contains(join.Type, StringComparer.OrdinalIgnoreCase)) continue;
                query.Append($" {join.Type} JOIN {join.RightCollectionName} ON {join.LeftCollectionName}.{join.LeftOnField} = {join.RightCollectionName}.{join.RightOnField}");
            }

            return query;
        }

        private StringBuilder MakeSelectedFieldsQueryPart(List<SelectQuerySelectedField> selctedFields)
        {
            var query = new StringBuilder("SELECT ");

            if (selctedFields.Any())
            {
                foreach (var field in selctedFields)
                {
                    query.Append($"{field.CollectionName}.{field.FieldName}, ");
                }

                query.Remove(query.Length - 2, 2);
            }
            else
            {
                query.Append('*');
            }

            return query;
        }

        private async Task<bool> CheckIfCollectionsAndColumnsExistInDatabase(
            List<SelectFiltersField> fieldsFilters,
            List<SelectQuerySelectedField> selctedFields,
            List<SelectQueryJoin> joins,
            string fromCollectionName,
            List<SelectQueryOrder> orders
            )
        {
            var collectionAndFieldsToCheck = new List<CollectionAndField>();

            var mappedJoins = joins.SelectMany(join => new List<CollectionAndField>
            {
                new CollectionAndField
                {
                    CollectionName = join.LeftCollectionName,
                    FieldName = join.LeftOnField
                },
                new CollectionAndField
                {
                    CollectionName = join.RightCollectionName,
                    FieldName = join.RightOnField
                }

            });
            collectionAndFieldsToCheck.AddRange(mappedJoins);

            var mappedSelectedFields = selctedFields.Select(field => new CollectionAndField
            {
                FieldName = field.FieldName,
                CollectionName = field.CollectionName
            });
            collectionAndFieldsToCheck.AddRange(mappedSelectedFields);

            var mappedFieldsFilters = fieldsFilters.Select(field => new CollectionAndField
            {
                CollectionName = field.CollectionName,
                FieldName = field.FieldName
            });
            collectionAndFieldsToCheck.AddRange(mappedFieldsFilters);

            collectionAndFieldsToCheck.Add(new CollectionAndField
            {
                CollectionName = fromCollectionName,
                FieldName = string.Empty
            });

            var mappeOrders = orders.Select(field => new CollectionAndField
            {
                CollectionName = field.CollectionName,
                FieldName = field.FieldName
            });
            collectionAndFieldsToCheck.AddRange(mappeOrders);

            var collectionAndFieldsToCheckGrouped = collectionAndFieldsToCheck
                                                        .Distinct()
                                                        .GroupBy(field => field.CollectionName,
                                                                field => field.FieldName)
                                                        .Select(groupedFields => new CollectionWithColumnsNames()
                                                        {
                                                            CollectionName = groupedFields.Key,
                                                            ColumnsNames = groupedFields.ToList()
                                                        });


            foreach (var collection in collectionAndFieldsToCheckGrouped)
            {
                var collectionFromDB = await GetCollectionFieldsByCollectionNameAsync(collection.CollectionName);

                if (collectionFromDB == null) return false;

                var fieldsNamesFromDB = collectionFromDB.Select(field => field.Name).ToList();

                var areAllFieldsExistInDbOrInReservedFields = collection.ColumnsNames.All(fieldName =>
                {
                    if (fieldName == string.Empty || fieldName == SelectAllSign.SIGN) return true;

                    return DataCollectionReservedFields.ReservedFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase) ||
                    fieldsNamesFromDB.Contains(fieldName, StringComparer.OrdinalIgnoreCase);
                });

                if (!areAllFieldsExistInDbOrInReservedFields) return false;
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
                _ => throw new Exception("Unknown operation"),
            };
        }

        private QueryAndParameters MakeFilterQueryPart(List<SelectFiltersField> fieldsFilters)
        {
            var query = new StringBuilder(" WHERE ");
            var parameters = new DynamicParameters();
            string? previousOperation = null;

            foreach (var field in fieldsFilters)
            {
                foreach (var filter in field.Filters)
                {
                    var operationSign = PrepareOperationSign(previousOperation, field.Operation);
                    var filterSign = SelectFiltersMapper.MapFilterToSign[filter.Type];

                    var parameterName = Guid.NewGuid().ToString("N");

                    query.Append($" {operationSign} {field.CollectionName}.{field.FieldName} {filterSign} @{parameterName} ");

                    if (filter.Type == SelectQueryFilters.IN || filter.Type == SelectQueryFilters.NOT_IN)
                    {
                       var value = JsonConvert.DeserializeObject<string[]>(filter.Value);
                       parameters.Add(parameterName, value);
                    }
                    else
                    {
                        parameters.Add(parameterName, filter.Value);
                    }
                }
                previousOperation = field.Operation;
            }

            query.Append(')');

            return new QueryAndParameters
            {
                Query = query,
                Parameters = parameters,
            };
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
