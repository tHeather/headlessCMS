using headlessCMS.Models.Services.SelectQuery;

namespace headlessCMS.Mappers
{
    public static class DraftApiRequestMapper
    {
        private const string DbSchema = "draft"; 

        private static string AddDbSchemaPrefix(string table)
        {
            return $"{DbSchema}.{table}";
        }

        public static SelectQueryParametersDataCollection SelectQuery(
            SelectQueryParametersDataCollection parameters)
        {

            parameters.From = AddDbSchemaPrefix(parameters.From);

            foreach (var field in parameters.SelctedFields)
            {
                field.CollectionName = AddDbSchemaPrefix(field.CollectionName);
            }

            foreach (var join in parameters.Joins)
            {
                join.LeftCollectionName = AddDbSchemaPrefix(join.LeftCollectionName);
                join.RightCollectionName = AddDbSchemaPrefix(join.RightCollectionName);
            }

            foreach (var filter in parameters.FieldsFilters)
            {
                filter.CollectionName = AddDbSchemaPrefix(filter.CollectionName);
            }

            foreach (var order in parameters.Orders)
            {
                order.CollectionName = AddDbSchemaPrefix(order.CollectionName);
            }

            return parameters;
        }
    }
}
