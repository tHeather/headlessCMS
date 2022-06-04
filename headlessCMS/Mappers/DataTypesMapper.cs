using headlessCMS.Constants;
using System.Data;

namespace headlessCMS.Dictionary
{
    public class DataTypesMapper
    {
        public static readonly Dictionary<string, string> MapToDatabaseType = new()
             {
                { DataTypes.STRING, DatabaseDataType.STRING },
                { DataTypes.INT, DatabaseDataType.INT },
                { DataTypes.BOOL, DatabaseDataType.BOOL },
                { DataTypes.DATE, DatabaseDataType.DATE },
        };

        public static readonly Dictionary<string, string> MapFromDatabaseType = new()
             {
                { DatabaseDataType.STRING, DataTypes.STRING },
                { DatabaseDataType.INT, DataTypes.INT },
                { DatabaseDataType.BOOL, DataTypes.BOOL },
                { DatabaseDataType.DATE, DataTypes.DATE },
        };

        public static readonly Dictionary<string, DbType> MapDatabaseTypeToDapper = new()
             {
                { DatabaseDataType.STRING, DbType.String },
                { DatabaseDataType.INT, DbType.Int32 },
                { DatabaseDataType.BOOL, DbType.Boolean },
                { DatabaseDataType.DATE, DbType.DateTime },
        };

    }
}

