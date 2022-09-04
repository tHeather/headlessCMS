using Dapper;
using headlessCMS.Constants;
using headlessCMS.Constants.TablesMetadata;
using headlessCMS.Models.Models;
using headlessCMS.Models.Services;
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

        protected async Task<bool> AreCollectionsAndColumnsExistInDatabaseAsync(IEnumerable<CollectionWithColumnsNames> collectionWithColumnsNames)
        {
            foreach (var collection in collectionWithColumnsNames)
            {
                var collectionFromDB = await GetCollectionFieldsByCollectionNameAsync(collection.CollectionName);

                if (collectionFromDB == null) return false;

                var fieldsNamesFromDB = collectionFromDB.Select(field => field.Name).ToList();

                var areAllFieldsExistInDbOrInReservedFields = collection.ColumnsNames.All(fieldName =>
                {
                    if (fieldName == string.Empty || fieldName == SelectAllSign.SIGN) return true;

                    return DataCollectionReservedFields.ReservedFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase) ||
                    fieldsNamesFromDB.Contains(fieldName, StringComparer.OrdinalIgnoreCase);
                });

                if (!areAllFieldsExistInDbOrInReservedFields) return false;
            }

            return true;
        }

        protected async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionNameAsync(string collectionName)
        {
            var query = $"SELECT * FROM {ReservedTables.COLLECTION_FIELDS} WHERE collectionName = @collectionName;";

            return await _dbConnection.QueryAsync<CollectionField>(query, new { collectionName });
        }
    }
}
