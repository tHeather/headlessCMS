using headlessCMS.Constants;

namespace headlessCMS.Mappers
{
    public class SelectFiltersMapper
    {
        public static readonly Dictionary<string, string> MapFilterToSign = new()
        {
                { SelectFilters.EQUAL, "=" },
                { SelectFilters.NOT_EQUAL, "!=" },
                { SelectFilters.GREATER_THAN, ">" },
                { SelectFilters.GREATER_THAN_OR_EQUAL, ">=" },
                { SelectFilters.LESS_THAN, "<" },
                { SelectFilters.LESS_THAN_OR_EQUAL, "<=" },
                { SelectFilters.IN, "IN" },
                { SelectFilters.NOT_IN, "NOT IN" },
        };

    }
}
