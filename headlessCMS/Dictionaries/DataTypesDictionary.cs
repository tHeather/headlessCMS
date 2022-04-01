using headlessCMS.Constants;

namespace headlessCMS.Dictionary
{
    public class DataTypesDictionary
    {
        public static readonly Dictionary<string, string> MapToDatabaseType = new Dictionary<string, string>()
             {
                { DataTypes.STRING, DatabaseDataType.STRING },
                { DataTypes.INT, DatabaseDataType.INT },
                { DataTypes.BOOL, DatabaseDataType.BOOL },
                { DataTypes.CHAR, DatabaseDataType.CHAR },
                { DataTypes.DATE, DatabaseDataType.DATE },
             };

        public static readonly Dictionary<string, string> MapFromDatabaseType = new Dictionary<string, string>()
             {
                { DatabaseDataType.STRING, DataTypes.STRING },
                { DatabaseDataType.INT, DataTypes.INT },
                { DatabaseDataType.BOOL, DataTypes.BOOL },
                { DatabaseDataType.CHAR, DataTypes.CHAR },
                { DatabaseDataType.DATE, DataTypes.DATE },
             };
    }
}

