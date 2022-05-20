using headlessCMS.Services.Interfaces;
using headlessCMS.Dictionary;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Transactions;
using headlessCMS.Models.Models;
using headlessCMS.Models.ValueObjects;

namespace headlessCMS.Services
{
    public class CollectionMetadataService: ICollectionMetadataService
    {

        private readonly SqlConnection _dbConnection;

        public CollectionMetadataService(SqlConnection connection)
        {
            _dbConnection = connection;
        }

        public async Task CreateCollection(CreateCollection createCollection)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var mappedFieldsAndTypes = new Dictionary<string,string>();

            var query = new StringBuilder(
            @$"CREATE TABLE {createCollection.Name} 
               (id UNIQUEIDENTIFIER NOT NULL DEFAULT newid(), dataState INT NOT NULL, publishedVersionId UNIQUEIDENTIFIER,"
            );

            foreach (var field in createCollection.Fields)
            {
                var mappedFieldType = DataTypesMapper.MapToDatabaseType[field.FieldType.ToUpper()];
                var nullability  = field.IsRequierd ? "NOT NULL":"";
                query.Append($"{field.Name} {mappedFieldType} {nullability},");
                mappedFieldsAndTypes.Add(field.Name, mappedFieldType);
            }

            query.Append(");");

            await _dbConnection.QueryAsync(query.ToString());
            await AddCollection(createCollection.Name);
            await AddCollectionFields(createCollection.Name, mappedFieldsAndTypes);

            transactionScope.Complete();
        }

        public async Task<IEnumerable<string>> GetCollectionsNames()
        {
          return await _dbConnection.QueryAsync<string>("SELECT collectionName FROM collections");
        }

        private async Task AddCollection(string collectionName)
        {
           await _dbConnection.ExecuteAsync(@$"
                INSERT INTO collections (collectionName) 
                VALUES ('{collectionName}');
                ");
        }

        private async Task AddCollectionFields(
            string collectionName, Dictionary<string,string> mappedFieldsAndTypes
            )
        {
            var rows = new StringBuilder();

            foreach (var mappedFieldAndType in mappedFieldsAndTypes)
            {
                rows.Append(
                    $"('{collectionName}','{mappedFieldAndType.Key}','{mappedFieldAndType.Value}'),"
                    );
            }

            rows.Remove(rows.Length - 1, 1);

            await _dbConnection.ExecuteAsync(@$"
                   INSERT INTO collectionFields (collectionName, name, type) 
                   VALUES {rows};
               ");
        }

        public async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionName(string collectionName)
        {
            return await _dbConnection.QueryAsync<CollectionField>(
                $"SELECT * FROM collectionFields WHERE collectionName = '{collectionName}';"
                );
        }

    }
}