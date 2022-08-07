namespace headlessCMS.Constants.TablesMetadata
{
    public class DataCollectionReservedFields
    {
        public const string ID = "id";

        public const string DATA_STATE = "dataState";

        public const string PUBLISHED_VERSION_ID = "publishedVersionId";

        public static readonly List<string> ReservedFields= new()
        {
            ID, DATA_STATE, PUBLISHED_VERSION_ID
        };
    }
}
