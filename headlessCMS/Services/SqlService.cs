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

        protected async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, DynamicParameters parameters)
        {
            return await _dbConnection.QueryAsync<T>(query, parameters);
        }

        protected async Task<T> ExecuteScalarAsync<T>(string query, DynamicParameters parameters)
        {
            return await _dbConnection.ExecuteScalarAsync<T>(query, parameters);
        }

        protected async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionNameAsync(string collectionName)
        {
            var query = $"SELECT * FROM {ReservedTables.COLLECTION_FIELDS} WHERE collectionName = @collectionName;";

            return await _dbConnection.QueryAsync<CollectionField>(query, new { collectionName });
        }
    }
}
