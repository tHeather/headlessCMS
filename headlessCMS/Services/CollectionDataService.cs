using Dapper;
using headlessCMS.Constants;
using headlessCMS.Models.ValueObjects;
using headlessCMS.Services.Interfaces;
using System.Data.SqlClient;
using System.Text;

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

        public async Task InsertData(InsertData insertData)
        {
            var collectionFields = await _collectionMetadataService
                                        .GetCollectionFieldsByCollectionName(insertData.CollectionName);

            var values = new StringBuilder();
            var columns = new StringBuilder();

            foreach (var field in collectionFields)
            {
                var value = insertData.ColumnsWithValues.GetValueOrDefault(field.Name);
                var valueString = field.Type == DatabaseDataType.INT || field.Type == DatabaseDataType.BOOL ? value : $"'{value}'";
                values.Append(valueString);
                values.Append(',');
                columns.Append($"{field.Name},");
            }
            values.Remove(values.Length - 1, 1);
            columns.Remove(columns.Length - 1, 1);

            var query = $"INSERT INTO {insertData.CollectionName} ({columns}) VALUES ({values});";
            await _dbConnection.QueryAsync(query);
        }

        public async Task<IEnumerable<dynamic>> GetData(string collectionName)
        {
            return await _dbConnection.QueryAsync($"SELECT * FROM {collectionName};");
        }

        public async Task DeleteData(DeleteData deleteData)
        {
             await _dbConnection.ExecuteAsync(
                $"DELETE FROM {deleteData.CollectionName} WHERE Id = '{deleteData.RowId}';"
                );
        }

        public async Task UpdateData(UpdateData updateData)
        {
            var collectionFields = await _collectionMetadataService
                                        .GetCollectionFieldsByCollectionName(updateData.CollectionName);

            var values = new StringBuilder();

            foreach (var field in collectionFields)
            {
                var value = updateData.ColumnsWithValues.GetValueOrDefault(field.Name);
                var valueString = field.Type == DatabaseDataType.INT || field.Type == DatabaseDataType.BOOL ? value : $"'{value}'";
                values.Append($"{field.Name}={valueString},");
            }
            values.Remove(values.Length - 1, 1);
    

            var query = @$"
                            UPDATE {updateData.CollectionName} 
                            SET {values}
                            WHERE Id = '{updateData.RowId}'; ";
            await _dbConnection.QueryAsync(query);
        }

    }
}
