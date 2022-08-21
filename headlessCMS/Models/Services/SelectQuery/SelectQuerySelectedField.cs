using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQuerySelectedField
    {
        [Required]
        public string CollectionName { get; set; }

        [Required]
        public string FieldName { get; set; }
    }
}
