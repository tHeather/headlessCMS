﻿using headlessCMS.Services.Interfaces;
using headlessCMS.Models.DTOs;
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
            $"CREATE TABLE {createCollection.Name} (Id UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),"
            );

            foreach (var field in createCollection.Fields)
            {
                var mappedFieldType = DataTypesDictionary.MapToDatabaseType[field.Value.ToUpper()];
                query.Append($"{field.Key} {mappedFieldType},");
                mappedFieldsAndTypes.Add(field.Key, mappedFieldType);
            }

            query.Append(");");

            await _dbConnection.QueryAsync(query.ToString());
            var collectionId = await AddCollection(createCollection.Name);
            await AddCollectionFields(collectionId, mappedFieldsAndTypes);

            transactionScope.Complete();
        }

        private async Task<Guid> AddCollection(string collectionName)
        {
           var id = await _dbConnection.ExecuteScalarAsync<Guid>(@$"
               INSERT INTO collections (collectionName) 
               OUTPUT INSERTED.Id
               VALUES ('{collectionName}');
               ");

            return id;
        }

        private async Task AddCollectionFields(
            Guid collectionId, Dictionary<string,string> mappedFieldsAndTypes
            )
        {
            var rows = new StringBuilder();

            foreach (var mappedFieldAndType in mappedFieldsAndTypes)
            {
                rows.Append(
                    $"(newid(),'{collectionId}','{mappedFieldAndType.Key}','{mappedFieldAndType.Value}'),"
                    );
            }

            rows.Remove(rows.Length - 1, 1);

            await _dbConnection.ExecuteAsync(@$"
                   INSERT INTO collectionFields (Id,collectionId,name,type) 
                   VALUES {rows};
               ");
        }

        public async Task<IEnumerable<CollectionField>> GetCollectionFieldsByCollectionName(string name)
        {
            var id = await GetCollectionIdByCollectionName(name);

            return await _dbConnection.QueryAsync<CollectionField>(
                $"SELECT * FROM collectionFields WHERE collectionId = '{id}';"
                );
        }

        public async Task<Guid> GetCollectionIdByCollectionName(string name)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<Guid>(
                $"SELECT id FROM collections WHERE collectionName = '{name}';"
                );
        }

    }
}