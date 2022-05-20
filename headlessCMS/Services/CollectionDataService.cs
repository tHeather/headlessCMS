using Dapper;
using headlessCMS.Constants;
using headlessCMS.Enums;
using headlessCMS.Mappers;
using headlessCMS.Models.ValueObjects;
using headlessCMS.Services.Interfaces;
using headlessCMS.Tools;
using System.Data.SqlClient;
using System.Transactions;

namespace headlessCMS.Services
{
    public class CollectionDataService: ICollectionDataService
    {
        private readonly SqlConnection _dbConnection;
        private readonly ICollectionMetadataService _collectionMetadataService;

        public CollectionDataService(
                SqlConnection connection,
                ICollectionMetadataService collectionMetadataService
            )
        {
            _dbConnection = connection;
            _collectionMetadataService = collectionMetadataService;
        }

        public async Task<Guid> SaveDraft(InsertData insertData)
        {
            var collectionFields = await _collectionMetadataService
                                        .GetCollectionFieldsByCollectionName(insertData.CollectionName);

            var columnsAndValues = SQLQueryMaker.MakeInsertValuesStrings(collectionFields, insertData.ColumnsWithValues);

            var query = @$"INSERT INTO {insertData.CollectionName} 
                           ({ReservedColumns.DATA_STATE}, {ReservedColumns.PUBLISHED_VERSION_ID}, {columnsAndValues.Columns}) 
                           OUTPUT inserted.id
                           VALUES ({(int)DataStates.Draft}, {DatabaseDataType.NULL}, {columnsAndValues.Values});";

            var draftId = await _dbConnection.ExecuteScalarAsync<Guid>(query);

            return draftId;
        }

        public async Task<Guid?> PublishData(Guid draftId, string collectionName)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Guid? publishedDataId = null;
            dynamic draftDynamic  = await _dbConnection
                                                .QueryFirstAsync(@$"SELECT * 
                                                                    FROM {collectionName} 
                                                                    WHERE id = '{draftId}' 
                                                                     AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Draft};");

            var draft = draftDynamic as IDictionary<string, object>;

            if (draft == null) return publishedDataId; 

            var columnsWithValues  = DynamicMapper.DynamicToColumnsWithValuesList(draftDynamic);
            var collectionFields = await _collectionMetadataService.GetCollectionFieldsByCollectionName(collectionName);

            if(draft[ReservedColumns.PUBLISHED_VERSION_ID] != null)
            {
                var values = SQLQueryMaker.MakeUpdateValuesString(collectionFields, columnsWithValues);

                var publishedUpdateQuery = @$"UPDATE {collectionName} 
                                              SET {values}
                                              WHERE Id = '{draft[ReservedColumns.PUBLISHED_VERSION_ID]}'
                                                AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Published};";

                await _dbConnection.ExecuteAsync(publishedUpdateQuery);

                publishedDataId = (Guid)draft[ReservedColumns.PUBLISHED_VERSION_ID];
            }
            else
            {
                var columnsAndValues = SQLQueryMaker.MakeInsertValuesStrings(collectionFields, columnsWithValues);

                var publishedInsertQuery = @$"INSERT INTO {collectionName} 
                                              ({ReservedColumns.DATA_STATE}, {ReservedColumns.PUBLISHED_VERSION_ID}, {columnsAndValues.Columns}) 
                                              OUTPUT inserted.id
                                              VALUES ({(int)DataStates.Published}, {DatabaseDataType.NULL}, {columnsAndValues.Values});";

                var publishedId = await _dbConnection.ExecuteScalarAsync<Guid>(publishedInsertQuery);

                var draftQuery = @$"UPDATE {collectionName} 
                                    SET {ReservedColumns.PUBLISHED_VERSION_ID} = '{publishedId}' 
                                    WHERE id = '{draftId}'
                                     AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Draft}";

                await _dbConnection.ExecuteAsync(draftQuery);

                publishedDataId = publishedId;
            }

            transactionScope.Complete();

            return publishedDataId;
        }

        public async Task SaveDraftAndPublishData(InsertData insertData)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var draftId = await SaveDraft(insertData);
            await PublishData(draftId, insertData.CollectionName);

            transactionScope.Complete();
        }

        public async Task UpdateDraft(UpdateData updateData)
        {
            var collectionFields = await _collectionMetadataService
                                        .GetCollectionFieldsByCollectionName(updateData.CollectionName);

            var values = SQLQueryMaker.MakeUpdateValuesString(collectionFields, updateData.ColumnsWithValues);

            var query = @$"UPDATE {updateData.CollectionName} 
                           SET {values}
                           WHERE Id = '{updateData.RowId}'
                            AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Draft}";

            await _dbConnection.QueryAsync(query);
        }

        public async Task UpdateDraftAndPublishData(UpdateData updateData)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await UpdateDraft(updateData);
            await PublishData(updateData.RowId, updateData.CollectionName);

            transactionScope.Complete();
        }

        public async Task UnpublishData(Guid publishedVersionId, string collectionName)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _dbConnection.QueryAsync(@$"DELETE FROM {collectionName} 
                                              WHERE Id = '{publishedVersionId}' 
                                                AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Published};");

            await _dbConnection.QueryAsync(@$"UPDATE {collectionName} 
                                              SET {ReservedColumns.PUBLISHED_VERSION_ID} = NULL 
                                              WHERE {ReservedColumns.PUBLISHED_VERSION_ID} = '{publishedVersionId}' 
                                                AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Draft};");

            transactionScope.Complete();
        }

        public async Task<IEnumerable<dynamic>> GetData(string collectionName, DataStates dataDtate)
        {
            return await _dbConnection.QueryAsync(@$"SELECT * 
                                                     FROM {collectionName} 
                                                     WHERE  {ReservedColumns.DATA_STATE} = {(int)dataDtate};");
        }

        public async Task DeleteData(DeleteData deleteData)
        {

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            IDictionary<string, object> publishedVersionId = await _dbConnection
                                                                    .QueryFirstAsync(@$"SELECT {ReservedColumns.PUBLISHED_VERSION_ID} 
                                                                                        FROM {deleteData.CollectionName} 
                                                                                        WHERE id = '{deleteData.DraftId}'
                                                                                            AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Draft};");

            await _dbConnection.QueryAsync(@$"DELETE FROM {deleteData.CollectionName} 
                                              WHERE Id = '{deleteData.DraftId}' 
                                                AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Draft};");


            if(publishedVersionId?[ReservedColumns.PUBLISHED_VERSION_ID] != null)
            {
                await _dbConnection.QueryAsync(@$"DELETE FROM {deleteData.CollectionName} 
                                                  WHERE Id = '{(Guid)publishedVersionId[ReservedColumns.PUBLISHED_VERSION_ID]}' 
                                                    AND {ReservedColumns.DATA_STATE} = {(int)DataStates.Published};");
            }


            transactionScope.Complete();

        }
    }
}
