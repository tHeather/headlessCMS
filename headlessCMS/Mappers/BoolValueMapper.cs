using headlessCMS.Constants;

namespace headlessCMS.Mappers
{
    public static class BoolValueMapper
    {
        public static int MapToInt(string value)
        {
           return value.ToLower() == "true" ? 1 : 0;
        }

        public static string MapToString(int value)
        {
            return value == 1 ? "true" : "false";
        }
    }
}
