namespace headlessCMS.Constants
{
    public static class ReservedColumns
    {
        public const string ID = "id";

        public const string DATA_STATE = "dataState";

        public const string PUBLISHED_VERSION_ID = "publishedVersionId";

        public static readonly List<string> ReservedColumnsList = new List<string> {
            ID, DATA_STATE, PUBLISHED_VERSION_ID
        };   
    }
}
