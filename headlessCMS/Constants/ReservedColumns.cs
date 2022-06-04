namespace headlessCMS.Constants
{
    public static class ReservedColumns
    {
        public const string ID = "id";

        public const string DATA_STATE = "dataState";

        public const string PUBLISHED_VERSION_ID = "publishedVersionId";

        public const string COLLECTION_NAME = "collectionName";

        public const string NAME = "name";

        public static readonly List<string> ReservedDataTableColumnsList = new List<string> {
            ID, DATA_STATE, PUBLISHED_VERSION_ID
        };   
    }
}
