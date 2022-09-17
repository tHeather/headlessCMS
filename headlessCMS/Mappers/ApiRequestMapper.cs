using headlessCMS.Models.Services.SelectQuery;

namespace headlessCMS.Mappers
{
    public static class ApiRequestMapper
    {
        private const string DraftDbSchema = "draft";

        public static string AddDraftDbSchemaPrefix(string table)
        {
            return $"{DraftDbSchema}.{table}";
        }

        public static string RemoveDBSchemaPrefix(string table)
        {
            var index = table.IndexOf('.');

            if (index >= 0)
            {
                return table.Substring(index + 1);
            }

            return table;
        }

        public static void AddDraftDbSchemaToSelectQuery(SelectQueryParametersDataCollection parameters)
        {
            parameters.From = AddDraftDbSchemaPrefix(parameters.From);

            foreach (var field in parameters.SelctedFields)
            {
                field.CollectionName = AddDraftDbSchemaPrefix(field.CollectionName);
            }

            foreach (var join in parameters.Joins)
            {
                join.LeftCollectionName = AddDraftDbSchemaPrefix(join.LeftCollectionName);
                join.RightCollectionName = AddDraftDbSchemaPrefix(join.RightCollectionName);
            }

            foreach (var filter in parameters.FieldsFilters)
            {
                filter.CollectionName = AddDraftDbSchemaPrefix(filter.CollectionName);
            }

            foreach (var order in parameters.Orders)
            {
                order.CollectionName = AddDraftDbSchemaPrefix(order.CollectionName);
            }
        }

        public static SelectQueryParametersDataCollection RemoveSchemaFromSelectQuery(
            SelectQueryParametersDataCollection parameters)
        {

            parameters.From = RemoveDBSchemaPrefix(parameters.From);

            foreach (var field in parameters.SelctedFields)
            {
                field.CollectionName = RemoveDBSchemaPrefix(field.CollectionName);
            }

            foreach (var join in parameters.Joins)
            {
                join.LeftCollectionName = RemoveDBSchemaPrefix(join.LeftCollectionName);
                join.RightCollectionName = RemoveDBSchemaPrefix(join.RightCollectionName);
            }

            foreach (var filter in parameters.FieldsFilters)
            {
                filter.CollectionName = RemoveDBSchemaPrefix(filter.CollectionName);
            }

            foreach (var order in parameters.Orders)
            {
                order.CollectionName = RemoveDBSchemaPrefix(order.CollectionName);
            }

            return parameters;
        }
    }
}
