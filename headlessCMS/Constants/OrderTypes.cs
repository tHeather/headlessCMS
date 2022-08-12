namespace headlessCMS.Constants
{
    public class OrderTypes
    {
        public const string ASC = "asc";

        public const string DESC = "desc";

        public static readonly List<string> OrderTypesList = new()
        {
            ASC, DESC
        };
    }
}
