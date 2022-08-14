namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQueryParametersDataCollection
    {
        public List<SelectQuerySelectedField> SelctedFields { get; set; }

        public string From { get; set; }

        public List<SelectQueryJoin> Joins { get; set; }

        public List<SelectFiltersField> FieldsFilters { get; set; }

        public List<SelectQueryOrder> Orders { get; set; }

        public SelectQueryPagination Pagination { get; set; }
    }
}