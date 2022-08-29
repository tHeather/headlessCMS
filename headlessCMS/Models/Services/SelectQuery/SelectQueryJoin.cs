using headlessCMS.Atributes;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQueryJoin
    {
        [Required]
        public string LeftCollectionName { get; set; }

        [Required]
        public string RightCollectionName { get; set; }

        [Required]
        public string LeftOnField { get; set; }

        [Required]
        public string RightOnField { get; set; }

        [Required]
        [JoinTypeValidation]
        public string Type { get; set; }
    }
}
