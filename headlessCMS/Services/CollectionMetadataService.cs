using headlessCMS.Services.Interfaces;
using headlessCMS.Dictionary;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Transactions;
using headlessCMS.Models.Models;
using headlessCMS.Models.ValueObjects;
using headlessCMS.Constants;
using headlessCMS.Models.Services;
using headlessCMS.Models.Shared;

namespace headlessCMS.Services
{
    public class CollectionMetadataService: ICollectionMetadataService
    {

        private readonly SqlConnection _dbConnection;
        private readonly ISqlService _sqlService;

        public CollectionMetadataService(SqlConnection connection, ISqlService sqlService)
        {
            _dbConnection = connection;
            _sqlService = sqlService;
        }

        public async Task CreateCollection(CreateCollection createCollection)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var mappedFieldsAndTypes = new Dictionary<string,string>();

            var query = new StringBuilder(
            @$"CREATE TABLE {createCollection.Name} 
               (id UNIQUEIDENTIFIER NOT NULL DEFAULT newid(), 
                {DataCollectionReservedFields.DATA_STATE} INT NOT NULL, 
                {DataCollectionReservedFields.PUBLISHED_VERSION_ID} UNIQUEIDENTIFIER,"
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
            var insertQueryParametersMetadataCollection = new InsertQueryParametersMetadataCollection() {
                CollectionName = ReservedTables.COLLECTIONS,
                DataToInsert = new List<List<ColumnWithValue>>()
                {
                    new List<ColumnWithValue>() 
                    { 
                        new ColumnWithValue
                        {
                            Name = CollectionsTableFields.COLLECTION_NAME,
                            Value = collectionName
                        } 
                    }
                } 
            };

            await _sqlService.ExecuteInsertQueryOnMetadataCollection(insertQueryParametersMetadataCollection);
        }

        private async Task AddCollectionFields(string collectionName, Dictionary<string, string> mappedFieldsAndTypes)
        {
          var rows = mappedFieldsAndTypes.Select(field => new List<ColumnWithValue>()
            {
                new ColumnWithValue()
                {
                    Name = CollectionFieldsTableFields.COLLECTION_NAME,
                    Value = collectionName
                },
                new ColumnWithValue()
                {
                    Name = CollectionFieldsTableFields.NAME,
                    Value = field.Key
                },
                new ColumnWithValue()
                {
                    Name = CollectionFieldsTableFields.TYPE,
                    Value = field.Value
                }
            }
            );

            var insertQueryParametersMetadataCollection = new InsertQueryParametersMetadataCollection()
            {
                CollectionName = ReservedTables.COLLECTION_FIELDS,
                DataToInsert = new List<List<ColumnWithValue>>(rows)
            };

            await _sqlService.ExecuteInsertQueryOnMetadataCollection(insertQueryParametersMetadataCollection);
        }

        public async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionName(string collectionName)
        {
            return await _dbConnection.QueryAsync<CollectionField>(
                $"SELECT * FROM {ReservedTables.COLLECTION_FIELDS} WHERE collectionName = '{collectionName}';"
                );
        }

    }
}