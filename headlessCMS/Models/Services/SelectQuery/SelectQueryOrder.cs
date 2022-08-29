using headlessCMS.Atributes;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQueryOrder
    {
        [Required]
        public string CollectionName { get; set; }

        [Required]
        [OrderTypeValidation]
        public string Type { get; set; }

        [Required]
        public string FieldName { get; set; }
    }
}
