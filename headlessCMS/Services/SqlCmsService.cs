using Dapper;
using headlessCMS.Constants.TablesMetadata;
using headlessCMS.Models.Services;
using headlessCMS.Services.Interfaces;
using System.Data.SqlClient;
using System.Text;

namespace headlessCMS.Services
{
    public class SqlCmsService : SqlService, ISqlCmsService
    {
        public SqlCmsService(SqlConnection connection) : base(connection)
        {
        }

        public async Task ExecuteInsertQueryAsync(InsertQueryParametersMetadataCollection insertQueryParameters)
        {
            var reservedTableFields = ReservedTables.GetReservedTableFields(insertQueryParameters.CollectionName);
            if (reservedTableFields == null) return;

            var values = new StringBuilder();
            var columns = string.Join(",", reservedTableFields);
            var parameters = new DynamicParameters();

            foreach (var (rowToInsert, index) in insertQueryParameters.DataToInsert.Select((rowToInsert, index) => (rowToInsert, index)))
            {
                values.Append('(');
                foreach (var fieldName in reservedTableFields)
                {
                    var value = rowToInsert.SingleOrDefault(d => d.Name == fieldName)?.Value;
                    if (value == null) continue;

                    parameters.Add($"@{index}{fieldName}", value);
                    values.Append($"@{index}{fieldName},");
                }
                if (values[^1] == '(')
                {
                    values.Remove(values.Length - 1, 1);
                }
                else
                {
                    var isLastRow = insertQueryParameters.DataToInsert.Count == index + 1;
                    values.Replace(",", isLastRow ? ")" : "),", values.Length - 1, 1);
                }
            }

            var query = @$"INSERT INTO {insertQueryParameters.CollectionName} ({columns}) VALUES {values};";

            await _dbConnection.ExecuteAsync(query, parameters);
        }
    }
}
