using Dapper;
using headlessCMS.Constants;
using headlessCMS.Enums;
using headlessCMS.Mappers;
using headlessCMS.Models.Services;
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
        private readonly ISqlService _sqlService;

        public CollectionDataService(
                SqlConnection connection,
                ICollectionMetadataService collectionMetadataService,
                ISqlService sqlService
            )
        {
            _dbConnection = connection;
            _collectionMetadataService = collectionMetadataService;
            _sqlService = sqlService;
        }

        public async Task<Guid> SaveDraft(InsertData insertData)
        {
            var insertQueryParameters = new InsertQueryParameters
            {
                CollectionName = insertData.CollectionName,
                DataState = DataStates.Draft,
                DataToInsert = insertData.ColumnsWithValues
            };

            var draftId = await _sqlService.ExecuteInsertQuery(insertQueryParameters);

            return draftId;
        }

        public async Task<Guid> PublishData(Guid draftId, string collectionName)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Guid publishedDataId = Guid.Empty;
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
                var publishedInsertQueryParameters = new InsertQueryParameters
                {
                    CollectionName = collectionName,
                    DataState = DataStates.Published,
                    DataToInsert = columnsWithValues
                };

                var publishedId = await _sqlService.ExecuteInsertQuery(publishedInsertQueryParameters);

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
