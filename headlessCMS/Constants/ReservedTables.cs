namespace headlessCMS.Constants
{
    public class ReservedTables
    {
        public const string RELATIONSHIPS = "relationships";

        public const string COLLECTIONS = "collections";

        public const string COLLECTION_FIELDS = "collectionFields";

        public static readonly List<string> ReservedTablesList = new List<string> {
            RELATIONSHIPS, COLLECTIONS, COLLECTION_FIELDS
        };
    }
}
