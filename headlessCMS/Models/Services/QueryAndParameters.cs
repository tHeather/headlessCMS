using Dapper;
using System.Text;

namespace headlessCMS.Models.Services
{
    public class QueryAndParameters
    {
        public StringBuilder Query { get; set; }

        public DynamicParameters Parameters { get; set; }
    }
}
