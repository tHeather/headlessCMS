using headlessCMS.Constants;

namespace headlessCMS.Mappers
{
    public class SelectFiltersMapper
    {
        public static readonly Dictionary<string, string> MapFilterToSign = new()
        {
                { SelectQueryFilters.EQUAL, "=" },
                { SelectQueryFilters.NOT_EQUAL, "!=" },
                { SelectQueryFilters.GREATER_THAN, ">" },
                { SelectQueryFilters.GREATER_THAN_OR_EQUAL, ">=" },
                { SelectQueryFilters.LESS_THAN, "<" },
                { SelectQueryFilters.LESS_THAN_OR_EQUAL, "<=" },
                { SelectQueryFilters.IN, "IN" },
                { SelectQueryFilters.NOT_IN, "NOT IN" },
        };

    }
}
