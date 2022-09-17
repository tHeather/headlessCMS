namespace headlessCMS.Constants.TablesMetadata
{
    public class DataCollectionReservedFields
    {
        public const string ID = "id";

        public const string PUBLISHED_VERSION_ID = "publishedVersionId";

        public static readonly List<string> ReservedFields= new()
        {
            ID, PUBLISHED_VERSION_ID
        };
    }
}
