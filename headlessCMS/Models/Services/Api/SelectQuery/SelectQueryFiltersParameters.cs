using headlessCMS.Atributes;
using headlessCMS.Constants;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectFiltersField
    {
        [Required]
        public string CollectionName { get; set; }

        [Required]
        public string FieldName { get; set; }

        [LogicalOperationValidation]
        public string Operation { get; set; } = LogicalOperations.AND;

        [Required]
        public List<SelectQueryFiltersFilter> Filters { get; set; }
    }
}
