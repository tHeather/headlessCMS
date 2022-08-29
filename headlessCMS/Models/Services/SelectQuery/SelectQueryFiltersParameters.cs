using headlessCMS.Atributes;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectFiltersField
    {
        [Required]
        public string CollectionName { get; set; }

        [Required]
        public string FieldName { get; set; }

        [Required]
        [LogicalOperationValidation]
        public string Operation { get; set; }

        [Required]
        public List<SelectQueryFiltersFilter> Filters { get; set; }
    }
}
