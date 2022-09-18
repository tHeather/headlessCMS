using Dapper;
using headlessCMS.Constants;
using headlessCMS.Mappers;
using headlessCMS.Models.Services;
using headlessCMS.Models.Services.SelectQuery;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text;

namespace headlessCMS.QueryMakers
{
    public static class SqlApiQueryMaker
    {
        public static string MakeInsertQuery(
            string collectionName, 
            IEnumerable<string> columns, 
            IEnumerable<string> values
        )
        {
            return @$"INSERT INTO {collectionName} ({string.Join(",", columns)}) 
                      OUTPUT inserted.id
                      VALUES ({string.Join(",",values)});";
        }

        public static string MakeInsertManyQuery(
            string collectionName,
            IEnumerable<string> columns,
            IEnumerable<IEnumerable<string>> valueRows
        )
        {
            var values = new StringBuilder();

            foreach (var row in valueRows)
            {
                values.Append($",({string.Join(",", row)})");
            }

            values.Remove(0,1);

            return @$"INSERT INTO {collectionName} ({string.Join(",", columns)}) 
                      OUTPUT inserted.id 
                      VALUES {values};";
        }

        public static StringBuilder MakePaginationPartForSelectQuery(SelectQueryPagination pagination)
        {
            var rowsToskip = pagination.PageSize * pagination.PageNumber;
            return new StringBuilder($" OFFSET {rowsToskip} ROWS FETCH NEXT {pagination.PageSize} ROWS ONLY");
        }

        public static StringBuilder MakeOrderPartForSelectQuery(List<SelectQueryOrder> orders)
        {
            var query = new StringBuilder(" ORDER BY");

            if (orders.Any())
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

        public static StringBuilder MakeFromPartForSelectQuery(string collectionName)
        {
            return new StringBuilder($" FROM {collectionName} ");
        }

        public static StringBuilder MakeJoinPartForSelectQuery(List<SelectQueryJoin> joins)
        {
            var query = new StringBuilder(" ");

            foreach (var join in joins)
            {
                if (!JoinTypes.JoinTypesList.Contains(join.Type, StringComparer.OrdinalIgnoreCase)) continue;
                query.Append($" {join.Type} JOIN {join.RightCollectionName} ON {join.LeftCollectionName}.{join.LeftOnField} = {join.RightCollectionName}.{join.RightOnField}");
            }

            return query;
        }

        public static StringBuilder MakeSelectedFieldsPartForSelectQuery(List<SelectQuerySelectedField> selctedFields)
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

        public static QueryAndParameters MakeFilterPartForSelectQuery(List<SelectFiltersField> fieldsFilters)
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

        private static string PrepareOperationSign(string? previousOperation, string currentOperation)
        {
            if (previousOperation == null) return "(";

            return currentOperation switch
            {
                LogicalOperations.AND => LogicalOperations.AND,
                LogicalOperations.OR => $") {LogicalOperations.OR} (",
                _ => throw new Exception("Unknown operation"),
            };
        }

    }
}
