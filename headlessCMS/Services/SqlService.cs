using Dapper;
using headlessCMS.Constants.TablesMetadata;
using headlessCMS.Models.Models;
using System.Data.SqlClient;

namespace headlessCMS.Services
{
    public abstract class SqlService
    {
        protected readonly SqlConnection _dbConnection;

        public SqlService(SqlConnection connection)
        {
            _dbConnection = connection;
        }

        protected async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionNameAsync(string collectionName)
        {
            var query = $"SELECT * FROM {ReservedTables.COLLECTION_FIELDS} WHERE collectionName = @collectionName;";

            return await _dbConnection.QueryAsync<CollectionField>(query, new { collectionName });
        }
    }
}
