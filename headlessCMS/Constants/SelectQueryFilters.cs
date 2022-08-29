namespace headlessCMS.Constants
{
    public class SelectQueryFilters
    {
        public const string EQUAL = "eq";

        public const string NOT_EQUAL = "ne";

        public const string GREATER_THAN = "gt";

        public const string GREATER_THAN_OR_EQUAL = "gte";

        public const string LESS_THAN = "lt";

        public const string LESS_THAN_OR_EQUAL = "lte";

        public const string IN = "in";

        public const string NOT_IN = "nin";

        public static readonly List<string> FilterTypesList = new()
        {
            EQUAL, NOT_EQUAL, GREATER_THAN, GREATER_THAN_OR_EQUAL,
            LESS_THAN, LESS_THAN_OR_EQUAL, IN, NOT_IN
        };
    }
}
