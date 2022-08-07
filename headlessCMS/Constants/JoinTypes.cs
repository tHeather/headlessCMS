namespace headlessCMS.Constants
{
    public class JoinTypes
    {
        public const string LEFT = "left";

        public const string RIGHT = "right";

        public const string INNER = "inner";

        public const string FULL_OUTER = "full outer";

        public static readonly List<string> JoinTypesList = new()
        {
            LEFT, RIGHT, INNER, FULL_OUTER
        };
    }
}
