namespace headlessCMS.Constants.TablesMetadata
{
    public class ReservedTables
    {
        public const string COLLECTIONS = "collections";

        public const string COLLECTION_FIELDS = "collectionFields";

        public static readonly List<string> ReservedTablesList = new List<string> {
            COLLECTIONS, COLLECTION_FIELDS
        };

        public static List<string>? GetReservedTableFields(string collectionName)
        {
            switch (collectionName)
            {
                case COLLECTIONS:
                    return CollectionsTableFields.ReserveFieldsList;

                case COLLECTION_FIELDS:
                    return CollectionFieldsTableFields.ReserveFieldsList;

                default:
                    return null;
            }
        }
    }
}
