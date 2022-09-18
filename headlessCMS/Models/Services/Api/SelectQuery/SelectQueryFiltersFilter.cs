using headlessCMS.Atributes;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQueryFiltersFilter
    {
        [Required]
        [SelectQueryFilterTypeValidation]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
