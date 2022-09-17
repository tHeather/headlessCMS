using Dapper;
using headlessCMS.Constants.TablesMetadata;
using headlessCMS.Enums;
using headlessCMS.Mappers;
using headlessCMS.Models.Services;
using headlessCMS.Models.Services.InsertQuery;
using headlessCMS.Models.Services.SelectQuery;
using headlessCMS.Models.ValueObjects;
using headlessCMS.Services.Interfaces;
using headlessCMS.Tools;
using System.Data.SqlClient;
using System.Transactions;

namespace headlessCMS.Services
{
    public class ApiService
    {
        private readonly SqlConnection _dbConnection;
        private readonly CollectionMetadataService _collectionMetadataService;
        private readonly ISqlApiService _sqlApiService;

        public ApiService(
                SqlConnection connection,
                CollectionMetadataService collectionMetadataService,
                ISqlApiService sqlApiService
            )
        {
            _dbConnection = connection;
            _collectionMetadataService = collectionMetadataService;
            _sqlApiService = sqlApiService;
        }

        public async Task<List<dynamic>> GetData(SelectQueryParametersDataCollection selectQueryParametersDataCollection)
        {
            return await _sqlApiService.ExecuteSelectQueryAsync(selectQueryParametersDataCollection);
        }

        public async Task<Guid> InsertPublishedDataAsync(InsertQueryParameters insertData)
        {
           return await _sqlApiService.InsertDataAsync(insertData);
        }

        //public async Task<Guid> InsertDraftDataAsync(InsertQueryParameters insertData)
        //{

           // add prefix to collectionName 

        //    return await _sqlApiService.InsertDataAsync(insertData);
        //}

        //public async Task<Guid> PublishDataAsync(Guid draftId, string collectionName)
        //{
        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    Guid publishedDataId = Guid.Empty;
        //    dynamic draftDynamic = await _dbConnection
        //                                        .QueryFirstAsync(@$"SELECT * 
        //                                                            FROM {collectionName} 
        //                                                            WHERE id = '{draftId}' 
        //                                                             AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Draft};");

        //    var draft = draftDynamic as IDictionary<string, object>;

        //    if (draft == null) return publishedDataId;

        //    var columnsWithValues = DynamicMapper.DynamicToColumnsWithValuesList(draftDynamic);
        //    var collectionFields = await _collectionMetadataService.GetCollectionFieldsByCollectionNameAsync(collectionName);

        //    if (draft[DataCollectionReservedFields.PUBLISHED_VERSION_ID] != null)
        //    {
        //        var values = SQLQueryMaker.MakeUpdateValuesString(collectionFields, columnsWithValues);

        //        var publishedUpdateQuery = @$"UPDATE {collectionName} 
        //                                      SET {values}
        //                                      WHERE Id = '{draft[DataCollectionReservedFields.PUBLISHED_VERSION_ID]}'
        //                                        AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Published};";

        //        await _dbConnection.ExecuteAsync(publishedUpdateQuery);

        //        publishedDataId = (Guid)draft[DataCollectionReservedFields.PUBLISHED_VERSION_ID];
        //    }
        //    else
        //    {
        //        var publishedInsertQueryParameters = new InsertQueryParameters
        //        {
        //            CollectionName = collectionName,
        //            DataState = DataState.Published,
        //            DataToInsert = columnsWithValues
        //        };

        //        var publishedId = await _sqlApiService.ExecuteInsertQueryAsync(publishedInsertQueryParameters);

        //        var draftQuery = @$"UPDATE {collectionName} 
        //                            SET {DataCollectionReservedFields.PUBLISHED_VERSION_ID} = '{publishedId}' 
        //                            WHERE id = '{draftId}'
        //                             AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Draft}";

        //        await _dbConnection.ExecuteAsync(draftQuery);

        //        publishedDataId = publishedId;
        //    }

        //    transactionScope.Complete();

        //    return publishedDataId;
        //}

        //public async Task SaveDraftAndPublishDataAsync(InsertData insertData)
        //{
        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    var draftId = await SaveDraftAsync(insertData);
        //    await PublishDataAsync(draftId, insertData.CollectionName);

        //    transactionScope.Complete();
        //}

        //public async Task UpdateDraftAsync(UpdateData updateData)
        //{
        //    var collectionFields = await _collectionMetadataService
        //                                .GetCollectionFieldsByCollectionNameAsync(updateData.CollectionName);

        //    var values = SQLQueryMaker.MakeUpdateValuesString(collectionFields, updateData.ColumnsWithValues);

        //    var query = @$"UPDATE {updateData.CollectionName} 
        //                   SET {values}
        //                   WHERE Id = '{updateData.RowId}'
        //                    AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Draft}";

        //    await _dbConnection.QueryAsync(query);
        //}

        //public async Task UpdateDraftAndPublishDataAsync(UpdateData updateData)
        //{
        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    await UpdateDraftAsync(updateData);
        //    await PublishDataAsync(updateData.RowId, updateData.CollectionName);

        //    transactionScope.Complete();
        //}

        //public async Task UnpublishDataAsync(Guid publishedVersionId, string collectionName)
        //{
        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    var deleteQueryParametersDataCollection = new DeleteQueryParametersDataCollection
        //    {
        //        CollectionName = collectionName,
        //        IdsAndDataStates = new List<IdAndDataState>
        //        {
        //            new IdAndDataState
        //            {
        //                Id = publishedVersionId,
        //                DataState = DataState.Published,
        //            }
        //        }
        //    };
        //    await _sqlApiService.ExecuteDeleteQueryAsync(deleteQueryParametersDataCollection);

        //    await _dbConnection.QueryAsync(@$"UPDATE {collectionName} 
        //                                      SET {DataCollectionReservedFields.PUBLISHED_VERSION_ID} = NULL 
        //                                      WHERE {DataCollectionReservedFields.PUBLISHED_VERSION_ID} = '{publishedVersionId}' 
        //                                        AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Draft};");

        //    transactionScope.Complete();
        //}

        //public async Task DeleteDataAsync(DeleteData deleteData)
        //{

        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    IDictionary<string, object> publishedVersionId = await _dbConnection
        //                                                            .QueryFirstAsync(@$"SELECT {DataCollectionReservedFields.PUBLISHED_VERSION_ID} 
        //                                                                                FROM {deleteData.CollectionName} 
        //                                                                                WHERE id = '{deleteData.DraftId}'
        //                                                                                    AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Draft};");

        //    await _dbConnection.QueryAsync(@$"DELETE FROM {deleteData.CollectionName} 
        //                                      WHERE Id = '{deleteData.DraftId}' 
        //                                        AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Draft};");


        //    if (publishedVersionId?[DataCollectionReservedFields.PUBLISHED_VERSION_ID] != null)
        //    {
        //        await _dbConnection.QueryAsync(@$"DELETE FROM {deleteData.CollectionName} 
        //                                          WHERE Id = '{(Guid)publishedVersionId[DataCollectionReservedFields.PUBLISHED_VERSION_ID]}' 
        //                                            AND {DataCollectionReservedFields.DATA_STATE} = {(int)DataState.Published};");
        //    }


        //    transactionScope.Complete();

        //}
    }
}
