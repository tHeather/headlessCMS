using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQueryParametersDataCollection
    {
        public List<SelectQuerySelectedField> SelctedFields { get; set; } = new();

        [Required]
        public string From { get; set; }

        public List<SelectQueryJoin> Joins { get; set; } = new ();

        public List<SelectFiltersField> FieldsFilters { get; set; } = new ();

        public List<SelectQueryOrder> Orders { get; set; } = new();

        public SelectQueryPagination? Pagination { get; set; }
    }
}