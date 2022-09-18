using Dapper;
using headlessCMS.Constants.TablesMetadata;
using headlessCMS.Models.Services;
using headlessCMS.Models.Services.InsertQuery;
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

        public async Task<Guid> InsertDataAsync(InsertQueryParameters queryParameters)
        {
            var collectionFields = await GetCollectionFieldsByCollectionNameAsync(queryParameters.CollectionName);
            if (!collectionFields.Any()) return Guid.Empty;

            var values = new List<string>();
            var columns = collectionFields.Select(f => f.Name);
            var parameters = new DynamicParameters();

            foreach (var field in collectionFields)
            {
                var value = queryParameters.DataToInsert.SingleOrDefault(d => d.Name == field.Name)?.Value;
                parameters.Add(field.Name, value);
                values.Add($"@{field.Name}");
            }

            var query = SqlApiQueryMaker.MakeInsertQuery(queryParameters.CollectionName, columns, values);
            return await _dbConnection.ExecuteScalarAsync<Guid>(query, parameters);
        }

        public async Task<IEnumerable<Guid>> InsertManyDataAsync(InsertManyQueryParameters queryParameters)
        {
            var collectionFields = await GetCollectionFieldsByCollectionNameAsync(queryParameters.CollectionName);
            if (!collectionFields.Any()) return new List<Guid>();

            var valueRows = new List<List<string>>();
            var columns = collectionFields.Select(f => f.Name);
            var parameters = new DynamicParameters();

            foreach (var row in queryParameters.DataToInsert)
            {
                var rowId = Guid.NewGuid().ToString("N");
                var valueRow = new List<string>();
                foreach (var field in collectionFields)
                {
                    var value = row.SingleOrDefault(d => d.Name == field.Name)?.Value;
                    parameters.Add($"{field.Name}{rowId}", value);
                    valueRow.Add($"@{field.Name}{rowId}");
                }
                valueRows.Add(valueRow);
            }

            var query = SqlApiQueryMaker.MakeInsertManyQuery(queryParameters.CollectionName, columns, valueRows);
            return await _dbConnection.QueryAsync<Guid>(query, parameters);
        }

        //public async Task<Guid> ExecuteDeleteQueryAsync(DeleteQueryParametersDataCollection deleteQueryParameters)
        //{
        //    var collectionFields = await GetCollectionFieldsByCollectionNameAsync(deleteQueryParameters.CollectionName);
        //    if (!collectionFields.Any()) return Guid.Empty;

        //    var conditions = new StringBuilder();
        //    var parameters = new DynamicParameters();

        //    foreach (var (idAndDataState, index) in deleteQueryParameters.IdsAndDataStates.Select((idAndDataState, index) => (idAndDataState, index)))
        //    {
        //        var idParameterName = $"@{index}idName";
        //        var dataStateParameterName = $"@{index}dataState";

        //        var separator = deleteQueryParameters.IdsAndDataStates.Count == index + 1 ? "" : "OR";
        //        conditions.Append(@$"({DataCollectionReservedFields.ID} = {idParameterName} AND {DataCollectionReservedFields.DATA_STATE} = {dataStateParameterName}) {separator}");
        //        parameters.Add(idParameterName, idAndDataState.Id, DbType.Guid);
        //        parameters.Add(dataStateParameterName, (int)idAndDataState.DataState, DbType.Int32);
        //    }

        //    return await _dbConnection.ExecuteScalarAsync<Guid>(@$"DELETE FROM {deleteQueryParameters.CollectionName}
        //                                                           OUTPUT deleted.Id
        //                                                           WHERE {conditions};", parameters);
        //}

        public async Task<List<dynamic>> ExecuteSelectQueryAsync(SelectQueryParametersDataCollection selectQueryParameters)
        {
            var collectionsAndFields = GetCollectionsAndFieldsFromQueryParameters(selectQueryParameters);

            if (! await AreCollectionsAndColumnsExistInDatabaseAsync(collectionsAndFields))
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

        private static IEnumerable<CollectionWithColumnsNames> GetCollectionsAndFieldsFromQueryParameters(
                SelectQueryParametersDataCollection queryParameters
            )
        {
            var collectionAndFieldsToCheck = new List<CollectionAndField>();

            var mappedJoins = queryParameters.Joins.SelectMany(join => new List<CollectionAndField>
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

            var mappedSelectedFields = queryParameters.SelctedFields.Select(field => new CollectionAndField
            {
                FieldName = field.FieldName,
                CollectionName = field.CollectionName
            });
            collectionAndFieldsToCheck.AddRange(mappedSelectedFields);

            var mappedFieldsFilters = queryParameters.FieldsFilters.Select(field => new CollectionAndField
            {
                CollectionName = field.CollectionName,
                FieldName = field.FieldName
            });
            collectionAndFieldsToCheck.AddRange(mappedFieldsFilters);

            collectionAndFieldsToCheck.Add(new CollectionAndField
            {
                CollectionName = queryParameters.From,
                FieldName = string.Empty
            });

            var mappeOrders = queryParameters.Orders.Select(field => new CollectionAndField
            {
                CollectionName = field.CollectionName,
                FieldName = field.FieldName
            });
            collectionAndFieldsToCheck.AddRange(mappeOrders);

            return collectionAndFieldsToCheck.Distinct()
                                             .GroupBy
                                             (
                                               field => field.CollectionName,
                                               field => field.FieldName
                                             )
                                             .Select(groupedFields => new CollectionWithColumnsNames()
                                             {
                                                CollectionName = groupedFields.Key,
                                                ColumnsNames = groupedFields.ToList()
                                             });


        }
    }
}
