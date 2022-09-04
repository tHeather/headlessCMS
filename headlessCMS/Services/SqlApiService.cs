using Dapper;
using headlessCMS.Constants;
using headlessCMS.Constants.TablesMetadata;
using headlessCMS.Dictionary;
using headlessCMS.Models.Services;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.QueryMakers;
using headlessCMS.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace headlessCMS.Services
{
    public class SqlApiService : SqlService, ISqlApiService
    {
        public SqlApiService(SqlConnection connection) : base(connection)
        {
        }

        public async Task<Guid> ExecuteInsertQueryAsync(InsertQueryParametersDataCollection insertQueryParameters)
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

        public async Task<Guid> ExecuteDeleteQueryAsync(DeleteQueryParametersDataCollection deleteQueryParameters)
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

        public async Task<List<dynamic>> ExecuteSelectQueryAsync(SelectQueryParametersDataCollection selectQueryParameters)
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

            var selectedFieldsQuery = SqlApiQueryMaker.MakeSelectedFieldsPartForSelectQuery(selectQueryParameters.SelctedFields);
            query.Append(selectedFieldsQuery);

            var fromQuery = SqlApiQueryMaker.MakeFromPartForSelectQuery(selectQueryParameters.From);
            query.Append(fromQuery);

            if (selectQueryParameters.Joins.Any())
            {
                var joinQuery = SqlApiQueryMaker.MakeJoinPartForSelectQuery(selectQueryParameters.Joins);
                query.Append(joinQuery);
            }

            if (selectQueryParameters.FieldsFilters.Any())
            {
                var filterQueryAndParameters = SqlApiQueryMaker.MakeFilterPartForSelectQuery(selectQueryParameters.FieldsFilters);
                query.Append(filterQueryAndParameters.Query);
                parameters.AddDynamicParams(filterQueryAndParameters.Parameters);
            }

            var orderQuery = SqlApiQueryMaker.MakeOrderPartForSelectQuery(selectQueryParameters.Orders);
            query.Append(orderQuery);

            if (selectQueryParameters.Pagination != null)
            {
                var paginationQuery = SqlApiQueryMaker.MakePaginationPartForSelectQuery(selectQueryParameters.Pagination);
                query.Append(paginationQuery);
            }

            var results = await _dbConnection.QueryAsync(query.ToString(), parameters);
            return results.ToList();
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
    }
}
